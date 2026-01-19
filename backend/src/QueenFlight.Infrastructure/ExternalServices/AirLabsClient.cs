using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QueenFlight.Infrastructure.ExternalServices;

public interface IAirLabsClient
{
    Task<JsonElement?> GetFlightDetailsAsync(string icao24);
    Task<JsonElement?> GetAirlinesAsync();
    Task<JsonElement?> GetAirportsAsync();
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

        // Clean construction: BaseUrl + Endpoint
        var url = $"{baseUrl}/flight?api_key={apiKey}&hex={icao24}";

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

    public async Task<JsonElement?> GetAirlinesAsync()
    {
        return await FetchBulkDataAsync("airlines");
    }

    public async Task<JsonElement?> GetAirportsAsync()
    {
        return await FetchBulkDataAsync("airports");
    }

    private async Task<JsonElement?> FetchBulkDataAsync(string endpoint)
    {
        var apiKey = _configuration["AirLabs:ApiKey"];
        var baseUrl = _configuration["AirLabs:BaseUrl"];
        
        // Clean construction
        var url = $"{baseUrl}/{endpoint}?api_key={apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"AirLabs Bulk Fetch ({endpoint}) failed: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            
            if (doc.RootElement.TryGetProperty("response", out var data) && data.ValueKind == JsonValueKind.Array)
            {
                return data.Clone();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception fetching AirLabs {endpoint}");
            return null;
        }
    }
}
