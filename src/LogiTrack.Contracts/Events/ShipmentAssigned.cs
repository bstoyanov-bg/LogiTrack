namespace LogiTrack.Contracts.Events
{
    public record ShipmentAssigned
    {
        public int ShipmentId { get; init; }
        public int DriverId { get; init; }
        public string TrackingNumber { get; init; } = "";
        public DateTime AssignedAtUtc { get; init; } = DateTime.UtcNow;
    }
}
