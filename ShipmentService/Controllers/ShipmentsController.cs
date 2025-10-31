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

        public ShipmentsController(AppDbContext dbContext, ShipmentCacheService cache)
        {
            _dbContext = dbContext;
            _cache = cache;
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
