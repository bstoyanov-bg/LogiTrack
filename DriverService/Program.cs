using DriverService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DriverManagerService>();
app.MapGet("/", () => "gRPC DriverService is running. Use a gRPC client to connect.");

app.Run();
