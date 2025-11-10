using DriverService.Data;
using DriverService.Models;
using DriverService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddGrpc();

// Add HealthChecks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<DriverDbContext>(name: "Database")
    .AddCheck("self", () => HealthCheckResult.Healthy());

// EF Core + SQL Server
builder.Services.AddDbContext<DriverDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DriverDatabase")));

// Register DriverManagerService as gRPC service (its implementation will use DbContext)
builder.Services.AddScoped<DriverManagerService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers();

// MassTransit - consumer that reacts to ShipmentAssigned
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ShipmentAssignedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("driverservice-shipment-assigned-queue", e =>
        {
            e.ConfigureConsumer<ShipmentAssignedConsumer>(context);
        });
    });
});

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

app.UseCors("AllowAngularClient");

// REST API endpoints
app.MapControllers();

// gRPC endpoints
app.MapGrpcService<DriverManagerService>();
app.MapGet("/", () => "gRPC DriverService is running. Use a gRPC client to connect.");

// Map HealthChecks endpoints
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/db", async (DriverDbContext db) =>
{
    try
    {
        await db.Database.CanConnectAsync();
        return Results.Ok(new { database = "Connected" });
    }
    catch
    {
        return Results.Problem("Database unreachable");
    }
});

app.Run();
