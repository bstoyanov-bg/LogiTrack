namespace LogiTrack.Contracts.Events
{
    public record DriverUpdated
    {
        public int DriverId { get; init; }
        public string Name { get; init; } = "";
        public string Status { get; init; } = "";
        public DateTime UpdatedAtUtc { get; init; } = DateTime.UtcNow;
    }
}
