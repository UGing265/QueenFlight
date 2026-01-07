using System.Text.Json;
using Microsoft.Extensions.Logging;
using QueenFlight.Core.Models;
using QueenFlight.Infrastructure.Interfaces;
using StackExchange.Redis;

namespace QueenFlight.Infrastructure.Services;

public class RedisFlightCache : IFlightCache
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisFlightCache> _logger;
    private const string KeyPrefix = "aircraft:";

    public RedisFlightCache(IConnectionMultiplexer redis, ILogger<RedisFlightCache> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task SetFlightAsync(FlightState flight)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(flight);
        // Set Expiry to 5 minutes (if no update, plane is assumed landed/out of range)
        await db.StringSetAsync($"{KeyPrefix}{flight.Icao24}", json, TimeSpan.FromMinutes(5));
    }

    public async Task<FlightState?> GetFlightAsync(string icao24)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync($"{KeyPrefix}{icao24}");
        
        if (json.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<FlightState>(json!);
    }

    public async Task<List<FlightState>> GetAllFlightsAsync()
    {
        var db = _redis.GetDatabase();
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        
        var keys = server.Keys(pattern: $"{KeyPrefix}*");
        var flights = new List<FlightState>();

        foreach (var key in keys)
        {
            var json = await db.StringGetAsync(key);
            if (!json.IsNullOrEmpty)
            {
                var flight = JsonSerializer.Deserialize<FlightState>(json!);
                if (flight != null) flights.Add(flight);
            }
        }

        return flights;
    }
}
