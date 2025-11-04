using Grpc.Core;

namespace DriverService.Services;

public class DriverManagerService : DriverManager.DriverManagerBase
{
    private readonly ILogger<DriverManagerService> _logger;
    public DriverManagerService(ILogger<DriverManagerService> logger)
    {
        _logger = logger;
    }

    public override Task<AssignShipmentReply> AssignShipment(AssignShipmentRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Assigning shipment {request.ShipmentId} to driver {request.DriverId}");
        return Task.FromResult(new AssignShipmentReply
        {
            Success = true,
            Message = $"Shipment {request.ShipmentId} assigned to driver {request.DriverId}"
        });
    }

    public override Task<UpdateDriverStatusReply> UpdateDriverStatus(UpdateDriverStatusRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Driver {request.DriverId} status updated to {request.Status}");
        return Task.FromResult(new UpdateDriverStatusReply
        {
            Success = true,
            Message = $"Driver {request.DriverId} is now {request.Status}"
        });
    }
}
