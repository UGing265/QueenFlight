using Microsoft.EntityFrameworkCore;
using QueenFlight.Infrastructure.Data;
using QueenFlight.Infrastructure.Interfaces;
using QueenFlight.Core.Interfaces;
using QueenFlight.Infrastructure.Services;
using StackExchange.Redis;
using QueenFlight.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
// Register Worker Service
builder.Services.AddHttpClient();
builder.Services.AddHostedService<FlightDataService>();

// Register Domain Services
builder.Services.AddScoped<IAirLabsClient, AirLabsClient>();
builder.Services.AddScoped<IFlightDetailsProvider, FlightDetailsProvider>();

// Register Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
builder.Services.AddSingleton<IFlightCache, RedisFlightCache>();

// Register SignalR
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    hubOptions.EnableDetailedErrors = true;
})
    .AddMessagePackProtocol();

builder.Services.AddSingleton<IFlightBroadcaster, QueenFlight.API.Services.SignalRFlightBroadcaster>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("http://localhost:3000") // Next.js default port
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// auto redirect to https if not using https
// app.UseHttpsRedirection();

app.UseCors("ClientPermission");

// Minimal API endpoints will be replaced by Controllers/Hubs later
app.MapControllers();
app.MapHub<QueenFlight.API.Hubs.FlightHub>("/flighthub");

app.Run();
