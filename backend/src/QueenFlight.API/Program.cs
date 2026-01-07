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

// Register Worker Service
builder.Services.AddHttpClient();
builder.Services.AddHostedService<FlightDataService>();

// Register Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
builder.Services.AddSingleton<IFlightCache, RedisFlightCache>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Minimal API endpoints will be replaced by Controllers/Hubs later
app.UseHttpsRedirection();

app.Run();
