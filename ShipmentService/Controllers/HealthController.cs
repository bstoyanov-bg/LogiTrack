using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace ShipmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IDistributedCache cache, ILogger<HealthController> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy", time = DateTime.UtcNow });
        }

        [HttpGet("redis")]
        public async Task<IActionResult> CheckRedis()
        {
            try
            {
                var testKey = "health:ping";
                await _cache.SetStringAsync(testKey, "pong");
                var value = await _cache.GetStringAsync(testKey);
                return Ok(new { redis = value == "pong" ? "Connected" : "Error" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis check failed");
                return StatusCode(500, new { redis = "Disconnected", error = ex.Message });
            }
        }
    }
}
