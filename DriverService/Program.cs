using DriverService.Data;
using DriverService.Models;
using DriverService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddGrpc();

// EF Core + SQL Server
builder.Services.AddDbContext<DriverDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DriverDatabase")));

// Register DriverManagerService as gRPC service (its implementation will use DbContext)
builder.Services.AddScoped<DriverManagerService>();

var app = builder.Build();

// Ensure DB is migrated + seed sample drivers
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DriverDbContext>();
    db.Database.Migrate(); // applies migrations (create DB if needed)

    // seed if empty
    if (!db.Drivers.Any())
    {
        db.Drivers.AddRange(new List<DriverEntity>
        {
            new DriverEntity { Name = "Ivan Todorov", Status = "Available" },
            new DriverEntity { Name = "Maria Ivanova", Status = "Busy" },
            new DriverEntity { Name = "Peter Georgiev", Status = "Available" },
        });
        db.SaveChanges();
    }
}

// Map gRPC service
app.MapGrpcService<DriverManagerService>();

// Add optional lightweight REST endpoint for testing / Angular
app.MapGet("/", () => "gRPC DriverService is running. Use a gRPC client to connect.");
app.MapGet("/api/drivers/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

app.Run();
