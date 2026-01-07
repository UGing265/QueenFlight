using Microsoft.EntityFrameworkCore;
using QueenFlight.Infrastructure.Data;
using QueenFlight.Infrastructure.Interfaces;
using QueenFlight.Infrastructure.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
// Register Worker Service
builder.Services.AddHttpClient();
builder.Services.AddHostedService<FlightDataService>();

// Register Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
builder.Services.AddSingleton<IFlightCache, RedisFlightCache>();

// Register SignalR
builder.Services.AddSignalR().AddJsonProtocol(options => {
    options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Keep PascalCase/camelCase as is or standard
});
builder.Services.AddSingleton<IFlightBroadcaster, QueenFlight.API.Services.SignalRFlightBroadcaster>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Minimal API endpoints will be replaced by Controllers/Hubs later
app.MapControllers();
app.MapHub<QueenFlight.API.Hubs.FlightHub>("/flighthub");

app.Run();
