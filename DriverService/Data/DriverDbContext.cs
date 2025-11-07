using DriverService.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverService.Data
{
    public class DriverDbContext : DbContext
    {
        public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
        {
        }

        public DbSet<DriverEntity> Drivers => Set<DriverEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriverEntity>(b =>
            {
                b.HasKey(d => d.Id);
                b.Property(d => d.Name).IsRequired().HasMaxLength(200);
                b.Property(d => d.Status).IsRequired().HasMaxLength(50);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
