using DriverService;
using LogiTrack.Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ShipmentService.Data;
using ShipmentService.Hubs;
using ShipmentService.Models;
using ShipmentService.Services;

namespace ShipmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ShipmentCacheService _cache;
        private readonly ILogger<ShipmentsController> _logger;
        private readonly DriverManager.DriverManagerClient _driverClient;
        private readonly IHubContext<ShipmentHub> _hubContext;
        private readonly IPublishEndpoint _publish;

        public ShipmentsController(AppDbContext dbContext, 
            ShipmentCacheService cache, 
            ILogger<ShipmentsController> logger, 
            DriverManager.DriverManagerClient driverClient,
            IHubContext<ShipmentHub> hubContext, 
            IPublishEndpoint publish)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
            _driverClient = driverClient;
            _hubContext = hubContext;
            _publish = publish;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cachedShipments = await _cache.GetAllShipmentsAsync();
            if (cachedShipments != null)
            {
                _logger.LogInformation("✅ Shipments loaded from cache");
                return Ok(cachedShipments);
            }

            var shipments = await _dbContext.Shipments.ToListAsync();
            _logger.LogInformation("📦 Shipments loaded from DB");

            await _cache.SetAllShipmentsAsync(shipments);

            return Ok(shipments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Try from cache
            var cachedShipment = await _cache.GetShipmentAsync(id);
            if (cachedShipment != null)
            {
                Response.Headers["X-Data-Source"] = "Cache";
                return Ok(cachedShipment);
            }

            // If not get from DB
            var shipment = await _dbContext.Shipments.FindAsync(id);
            if (shipment == null) return NotFound();

            // Write in cache
            await _cache.SetShipmentAsync(shipment);

            Response.Headers["X-Data-Source"] = "DB";
            return Ok(shipment);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Shipment shipment)
        {
            if (shipment == null)
            {
                return BadRequest("Shipment is null");
            }

            _dbContext.Shipments.Add(shipment);
            await _dbContext.SaveChangesAsync();

            // Request driver assignment dynamically via gRPC
            var assignRequest = new AssignShipmentRequest
            {
                ShipmentId = shipment.Id
            };

            var reply = await _driverClient.AssignShipmentAsync(assignRequest);
            _logger.LogInformation($"gRPC reply: {reply.Message}");

            await _hubContext.Clients.All.SendAsync("ShipmentCreated", shipment);

            // Store the new shipment in cache
            await _cache.SetShipmentAsync(shipment);

            await _cache.RemoveAllShipmentsAsync();

            return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Shipment updatedShipment)
        {
            var existing = await _dbContext.Shipments.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Update properties
            existing.TrackingNumber = updatedShipment.TrackingNumber;
            existing.Origin = updatedShipment.Origin;
            existing.Destination = updatedShipment.Destination;
            existing.Status = updatedShipment.Status;
            existing.CreatedAt = updatedShipment.CreatedAt;

            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ShipmentUpdated", existing);

            // Update cache entry
            await _cache.SetShipmentAsync(existing);

            await _cache.RemoveAllShipmentsAsync();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _dbContext.Shipments.FindAsync(id);
            if (shipment == null) return NotFound();

            _dbContext.Shipments.Remove(shipment);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ShipmentDeleted", id);

            // Remove it from cache
            await _cache.RemoveShipmentAsync(id);

            await _cache.RemoveAllShipmentsAsync();

            return NoContent();
        }

        [HttpPost("{id}/assign/{driverId}")]
        public async Task<IActionResult> Assign(int id, int driverId)
        {
            var shipment = await _dbContext.Shipments.FindAsync(id);
            if (shipment == null) return NotFound();

            // et assignment
            shipment.AssignedDriverId = driverId;
            shipment.Status = ShipmentStatus.Assigned;
            await _dbContext.SaveChangesAsync();

            // Publish event
            var evt = new ShipmentAssigned
            {
                ShipmentId = shipment.Id,
                DriverId = driverId,
                TrackingNumber = shipment.TrackingNumber,
                AssignedAtUtc = DateTime.UtcNow
            };

            await _publish.Publish(evt);

            // Update cache entry
            await _cache.SetShipmentAsync(shipment);
            // Broadcast the shipment change
            await _hubContext.Clients.All.SendAsync("ShipmentUpdated", shipment);

            return Ok(shipment);
        }
    }
}
