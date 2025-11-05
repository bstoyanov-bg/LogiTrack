using Grpc.Core;

namespace DriverService.Services;

public class DriverManagerService : DriverManager.DriverManagerBase
{
    // In-memory list simulating a database
    private static readonly List<Driver> _drivers = new()
        {
            new Driver { Id = 1, Name = "Ivan Todorov", Status = "Available" },
            new Driver { Id = 2, Name = "Maria Ivanova", Status = "Busy" },
            new Driver { Id = 3, Name = "Peter Georgiev", Status = "Available" }
        };

    private readonly ILogger<DriverManagerService> _logger;

    public DriverManagerService(ILogger<DriverManagerService> logger)
    {
        _logger = logger;
    }

    public override Task<AssignShipmentReply> AssignShipment(AssignShipmentRequest request, ServerCallContext context)
    {
        // Find first available driver
        var driver = _drivers.FirstOrDefault(d => d.Status == "Available");

        if (driver == null)
        {
            return Task.FromResult(new AssignShipmentReply
            {
                DriverId = 0,
                Message = "No available drivers at the moment."
            });
        }

        // Mark driver as busy
        driver.Status = "Busy";
        _logger.LogInformation($"Assigned shipment {request.ShipmentId} to driver {driver.Name}");

        return Task.FromResult(new AssignShipmentReply
        {
            DriverId = driver.Id,
            Message = $"Shipment {request.ShipmentId} assigned to driver {driver.Name}"
        });
    }

    public override Task<UpdateDriverStatusReply> UpdateDriverStatus(UpdateDriverStatusRequest request, ServerCallContext context)
    {
        var driver = _drivers.FirstOrDefault(d => d.Id == request.DriverId);
        if (driver == null)
        {
            return Task.FromResult(new UpdateDriverStatusReply { Message = "Driver not found." });
        }

        driver.Status = request.Status;
        _logger.LogInformation($"Driver {driver.Name} status updated to {request.Status}");

        return Task.FromResult(new UpdateDriverStatusReply { Message = "Driver status updated successfully." });
    }

    public override Task<RegisterDriverReply> RegisterDriver(RegisterDriverRequest request, ServerCallContext context)
    {
        var newDriver = new Driver
        {
            Id = _drivers.Max(d => d.Id) + 1,
            Name = request.Name,
            Status = "Available"
        };

        _drivers.Add(newDriver);
        _logger.LogInformation($"Registered new driver: {newDriver.Name}");

        return Task.FromResult(new RegisterDriverReply
        {
            DriverId = newDriver.Id,
            Message = $"Driver {newDriver.Name} registered successfully."
        });
    }

    public override Task<DriverListReply> GetAllDrivers(Empty request, ServerCallContext context)
    {
        var reply = new DriverListReply();
        reply.Drivers.AddRange(_drivers);
        return Task.FromResult(reply);
    }
}
