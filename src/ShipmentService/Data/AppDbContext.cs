using Microsoft.EntityFrameworkCore;
using ShipmentService.Models;

namespace ShipmentService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();
}
