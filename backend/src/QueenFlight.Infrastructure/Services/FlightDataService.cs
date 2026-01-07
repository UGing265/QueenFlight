using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using QueenFlight.Core.Interfaces;
using QueenFlight.Core.Models;
using QueenFlight.Infrastructure.Interfaces;

namespace QueenFlight.Infrastructure.Services;

public class FlightDataService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FlightDataService> _logger;
    private readonly IFlightCache _flightCache;
    private readonly IConfiguration _configuration;
    private readonly IFlightBroadcaster _broadcaster;
    
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public FlightDataService(
        IHttpClientFactory httpClientFactory, 
        ILogger<FlightDataService> logger, 
        IFlightCache flightCache,
        IConfiguration configuration,
        IFlightBroadcaster broadcaster)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _flightCache = flightCache;
        _configuration = configuration;
        _broadcaster = broadcaster;

        // Archon Resilience: Exponential Backoff for API stability
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning($"⚠️ AirLabs API failed (Attempt {retryCount}). Waiting {timeSpan.TotalSeconds}s...");
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 FlightDataService Started. Polling AirLabs every 600s...");

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(600));

        // Run immediately on start
        try { await FetchAndProcessData(stoppingToken); } catch (Exception ex) { _logger.LogError(ex, "Initial fetch failed"); }

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                await FetchAndProcessData(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Critical error in FlightDataService loop");
            }
        }
    }

    private async Task FetchAndProcessData(CancellationToken ct)
    {
        var apiKey = _configuration["AirLabs:ApiKey"];
        var baseUrl = _configuration["AirLabs:BaseUrl"];

        if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_"))
        {
            _logger.LogWarning("⚠️ AirLabs API Key is missing or default. Skipping poll.");
            return;
        }

        var client = _httpClientFactory.CreateClient();
        var url = $"{baseUrl}?api_key={apiKey}"; 
        // Note: For MVP we fetch 'global' or default set. 
        // AirLabs allows bounding box: &bbox=... but free tier might have restrictions. 
        // Ideally: &bbox=8.17,102.14,23.39,109.46 (Vietnam)

        try
        {
            var response = await _retryPolicy.ExecuteAsync(async () => 
                await client.GetAsync(url, ct));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"❌ Failed to fetch data: {response.StatusCode} - {await response.Content.ReadAsStringAsync(ct)}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            
            if (!doc.RootElement.TryGetProperty("response", out var flightsElement) || flightsElement.ValueKind != JsonValueKind.Array)
            {
                _logger.LogWarning("⚠️ No 'response' array found in AirLabs JSON.");
                return;
            }

            int count = 0;
            foreach (var element in flightsElement.EnumerateArray())
            {
                if (TryMapAirLabs(element, out var flight))
                {
                    // Validation: Must have Lat, Lng, Heading, Speed
                    if (flight.Latitude == null || flight.Longitude == null || 
                        flight.TrueTrack == null || flight.Velocity == null)
                    {
                        continue;
                    }

                    await _flightCache.SetFlightAsync(flight);
                    count++;
                }
            }

            _logger.LogInformation($"✅ AirLabs Snapshot: {count} valid flights synced to Redis.");

            // Broadcast to SignalR via Interface
            await _broadcaster.BroadcastFlightCountAsync(count);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error syncing with AirLabs");
        }
    }

    private bool TryMapAirLabs(JsonElement el, out FlightState flight)
    {
        flight = null!;
        try
        {
            // AirLabs structure: hex, lat, lng, dir, speed, alt, ...
            string hex = GetString(el, "hex");
            if (string.IsNullOrEmpty(hex)) return false;

            flight = new FlightState(
                Icao24: hex,
                Callsign: GetString(el, "flight_number") ?? GetString(el, "flight_icao") ?? GetString(el, "reg_number") ?? "N/A",
                OriginCountry: GetString(el, "flag"),
                Longitude: GetFloat(el, "lng"),
                Latitude: GetFloat(el, "lat"),
                BaroAltitude: GetFloat(el, "alt"),
                OnGround: GetString(el, "status") == "ground", // Heuristic
                Velocity: GetFloat(el, "speed"), // Unit: km/h (Confirmed by AirLabs)
                TrueTrack: GetFloat(el, "dir"),
                VerticalRate: GetFloat(el, "v_speed"),
                GeoAltitude: null,
                Squawk: GetString(el, "squawk"),
                Spi: false,
                PositionSource: 0
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string? GetString(JsonElement el, string prop) => 
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private float? GetFloat(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetSingle() : null;
}
