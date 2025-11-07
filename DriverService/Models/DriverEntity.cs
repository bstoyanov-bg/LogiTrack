namespace DriverService.Models
{
    public class DriverEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = "Available"; // Available, Busy, Offline, OnRoute
        public DateTime? LastSeenUtc { get; set; } // optional for future tracking
    }
}
