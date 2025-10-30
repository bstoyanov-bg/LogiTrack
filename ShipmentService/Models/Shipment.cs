using System.ComponentModel.DataAnnotations;

namespace ShipmentService.Models
{
    public enum ShipmentStatus
    {
        Created,
        InTransit,
        Delivered,
        Cancelled
    }

    public class Shipment
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string TrackingNumber { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Origin { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Destination { get; set; } = null!;
        [Required]
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
