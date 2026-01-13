using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QueenFlight.Infrastructure.ExternalServices;

public interface IAirLabsClient
{
    Task<JsonElement?> GetFlightDetailsAsync(string icao24);
}

public class AirLabsClient : IAirLabsClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AirLabsClient> _logger;

    public AirLabsClient(HttpClient httpClient, IConfiguration configuration, ILogger<AirLabsClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<JsonElement?> GetFlightDetailsAsync(string icao24)
    {
        var apiKey = _configuration["AirLabs:ApiKey"];
        var baseUrl = _configuration["AirLabs:BaseUrl"];

        // Note: AirLabs 'flight' endpoint gives details for a specific flight
        var url = $"{baseUrl}?api_key={apiKey}&hex={icao24}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"AirLabs call failed for {icao24}: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            // AirLabs format: { "response": [ { ... } ] }
            if (doc.RootElement.TryGetProperty("response", out var responseInfo) && 
                responseInfo.ValueKind == JsonValueKind.Array && 
                responseInfo.GetArrayLength() > 0)
            {
                // Return the first element (flight details)
                return responseInfo[0].Clone(); 
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception fetching AirLabs details for {icao24}");
            return null;
        }
    }
}
