using Microsoft.AspNetCore.Mvc;
using DriverService.Data;
using Microsoft.EntityFrameworkCore;

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
    }
}
