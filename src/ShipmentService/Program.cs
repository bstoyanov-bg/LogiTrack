using DriverService;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ShipmentService.Consumers;
using ShipmentService.Data;
using ShipmentService.Hubs;
using ShipmentService.Middleware;
using ShipmentService.Services;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // for SignalR
    });
});


builder.Services.AddScoped<ShipmentCacheService>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR
builder.Services.AddSignalR();

builder.Services.AddGrpcClient<DriverManager.DriverManagerClient>(o =>
{
    o.Address = new Uri("http://localhost:5084");
});

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

// MassTransit config
builder.Services.AddMassTransit(x =>
{
    // Consumer for DriverUpdated (so we can broadcast over SignalR)
    x.AddConsumer<DriverUpdatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("shipmentservice-driver-updated-queue", e =>
        {
            e.ConfigureConsumer<DriverUpdatedConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularClient");

// Custom middleware to measure request time
app.UseResponseTimeMiddleware();

// SignalR Hubs
app.MapHub<ShipmentHub>("/hub/shipments");

app.MapControllers();

app.Run();
