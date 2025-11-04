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


            // Симулираме назначаване на шофьор (driverId = 1)
            var assignRequest = new AssignShipmentRequest
            {
                DriverId = 1,
                ShipmentId = shipment.Id
            };

            var reply = await _driverClient.AssignShipmentAsync(assignRequest);
            _logger.LogInformation($"gRPC reply: {reply.Message}");


            await _cache.SetShipmentAsync(shipment);

            return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _dbContext.Shipments.FindAsync(id);
            if (shipment == null) return NotFound();

            _dbContext.Shipments.Remove(shipment);
            await _dbContext.SaveChangesAsync();

            await _cache.RemoveShipmentAsync(id);

            return NoContent();
        }
    }
}
