using DriverService;
using Microsoft.AspNetCore.Mvc;
using ShipmentService.Data;
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

        public ShipmentsController(AppDbContext dbContext, 
            ShipmentCacheService cache, 
            ILogger<ShipmentsController> logger, 
            DriverManager.DriverManagerClient driverClient)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
            _driverClient = driverClient;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Try from cache
            var cachedShipment = await _cache.GetShipmentAsync(id);
            if (cachedShipment != null)
                return Ok(cachedShipment);

            // If not get from DB
            var shipment = await _dbContext.Shipments.FindAsync(id);
            if (shipment == null) return NotFound();

            // Write in cache
            await _cache.SetShipmentAsync(shipment);

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

            // Store the new shipment in cache
            await _cache.SetShipmentAsync(shipment);

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

            // Update cache entry
            await _cache.SetShipmentAsync(existing);

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _dbContext.Shipments.FindAsync(id);
            if (shipment == null) return NotFound();

            _dbContext.Shipments.Remove(shipment);
            await _dbContext.SaveChangesAsync();

            // Remove it from cache
            await _cache.RemoveShipmentAsync(id);

            return NoContent();
        }
    }
}
