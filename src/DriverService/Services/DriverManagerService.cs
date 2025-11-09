using DriverService.Data;
using DriverService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DriverService.Services;

public class DriverManagerService : DriverManager.DriverManagerBase
{
    private readonly DriverDbContext _db;
    private readonly ILogger<DriverManagerService> _logger;

    public DriverManagerService(DriverDbContext db, ILogger<DriverManagerService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public override async Task<AssignShipmentReply> AssignShipment(AssignShipmentRequest request, ServerCallContext context)
    {
        // Find first available driver
        var driver = await _db.Drivers
            .Where(d => d.Status == "Available")
            .OrderBy(d => d.Id)
            .FirstOrDefaultAsync();

        if (driver == null)
        {
            return new AssignShipmentReply
            {
                DriverId = 0,
                Message = "No available drivers at the moment."
            };
        }

        // Mark driver as Busy and update LastSeen
        driver.Status = "Busy";
        driver.LastSeenUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Assigned shipment {ShipmentId} to driver {DriverId}", request.ShipmentId, driver.Id);

        return new AssignShipmentReply
        {
            DriverId = driver.Id,
            Message = $"Shipment {request.ShipmentId} assigned to driver {driver.Name}"
        };
    }

    public override async Task<UpdateDriverStatusReply> UpdateDriverStatus(UpdateDriverStatusRequest request, ServerCallContext context)
    {
        var driver = await _db.Drivers.FindAsync(request.DriverId);
        if (driver == null)
        {
            return new UpdateDriverStatusReply { Success = false, Message = "Driver not found." };
        }

        driver.Status = request.Status;
        driver.LastSeenUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Driver {DriverId} status updated to {Status}", driver.Id, request.Status);

        return new UpdateDriverStatusReply { Success = true, Message = "Driver status updated successfully." };
    }

    public override async Task<RegisterDriverReply> RegisterDriver(RegisterDriverRequest request, ServerCallContext context)
    {
        var driver = new DriverEntity
        {
            Name = request.Name,
            Status = "Available",
            LastSeenUtc = DateTime.UtcNow
        };

        _db.Drivers.Add(driver);

        await _db.SaveChangesAsync();

        _logger.LogInformation("Registered new driver: {Name} (Id={Id})", driver.Name, driver.Id);

        return new RegisterDriverReply
        {
            DriverId = driver.Id,
            Message = $"Driver {driver.Name} registered successfully."
        };
    }

    public override async Task<DriverListReply> GetAllDrivers(Empty request, ServerCallContext context)
    {
        var drivers = await _db.Drivers.ToListAsync();
        var reply = new DriverListReply();

        foreach (var d in drivers)
        {
            reply.Drivers.Add(new Driver
            {
                Id = d.Id,
                Name = d.Name,
                Status = d.Status
            });
        }
        return reply;
    }
}
