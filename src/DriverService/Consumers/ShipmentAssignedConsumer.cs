using DriverService.Data;
using LogiTrack.Contracts.Events;
using MassTransit;

namespace DriverService.Consumers
{
    public class ShipmentAssignedConsumer : IConsumer<ShipmentAssigned>
    {
        private readonly DriverDbContext _db;
        private readonly IPublishEndpoint _publish; // to publish DriverUpdated
        private readonly ILogger<ShipmentAssignedConsumer> _logger;

        public ShipmentAssignedConsumer(DriverDbContext db, IPublishEndpoint publish, ILogger<ShipmentAssignedConsumer> logger)
        {
            _db = db;
            _publish = publish;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ShipmentAssigned> context)
        {
            var evt = context.Message;
            _logger.LogInformation("Received ShipmentAssigned: {ShipmentId} -> Driver {DriverId}", evt.ShipmentId, evt.DriverId);

            var driver = await _db.Drivers.FindAsync(evt.DriverId);
            if (driver == null)
            {
                _logger.LogWarning("Driver {DriverId} not found", evt.DriverId);
                return;
            }

            driver.Status = "Busy";
            driver.LastSeenUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // publish DriverUpdated event
            var driverUpdated = new LogiTrack.Contracts.Events.DriverUpdated
            {
                DriverId = driver.Id,
                Name = driver.Name,
                Status = driver.Status,
                UpdatedAtUtc = DateTime.UtcNow
            };
            await _publish.Publish(driverUpdated);
        }
    }
}
