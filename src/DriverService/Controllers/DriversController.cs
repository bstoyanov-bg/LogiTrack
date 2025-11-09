using Microsoft.AspNetCore.Mvc;
using DriverService.Data;
using Microsoft.EntityFrameworkCore;
using DriverService.Models;

namespace DriverService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly DriverDbContext _db;
        private readonly ILogger<DriversController> _logger;

        public DriversController(DriverDbContext db, ILogger<DriversController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var drivers = await _db.Drivers
                .AsNoTracking()
                .ToListAsync();

            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null) return NotFound();

            return Ok(driver);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DriverEntity driver)
        {
            driver.Status = "Available";
            _db.Drivers.Add(driver);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Driver {driver.Name} registered.");
            return Ok(driver);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null)
                return NotFound();

            driver.Status = status;
            await _db.SaveChangesAsync();
            return Ok(driver);
        }
    }
}
