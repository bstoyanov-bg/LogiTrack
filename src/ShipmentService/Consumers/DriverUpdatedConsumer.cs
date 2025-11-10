using MassTransit;
using LogiTrack.Contracts.Events;
using Microsoft.AspNetCore.SignalR;
using ShipmentService.Hubs;

namespace ShipmentService.Consumers
{
    public class DriverUpdatedConsumer : IConsumer<DriverUpdated>
    {
        private readonly IHubContext<ShipmentHub> _hub;

        public DriverUpdatedConsumer(IHubContext<ShipmentHub> hub)
        {
            _hub = hub;
        }

        public async Task Consume(ConsumeContext<DriverUpdated> context)
        {
            var evt = context.Message;
            // Broadcast driver update to clients
            await _hub.Clients.All.SendAsync("DriverUpdated", new
            {
                driverId = evt.DriverId,
                name = evt.Name,
                status = evt.Status,
                updatedAt = evt.UpdatedAtUtc
            });
        }
    }
}
