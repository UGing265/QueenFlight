using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueenFlight.Core.Entities;
using QueenFlight.Infrastructure.ExternalServices;

namespace QueenFlight.Infrastructure.Data;

public class StaticDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IAirLabsClient _airLabsClient;
    private readonly ILogger<StaticDataSeeder> _logger;

    public StaticDataSeeder(
        AppDbContext dbContext,
        IAirLabsClient airLabsClient,
        ILogger<StaticDataSeeder> logger)
    {
        _dbContext = dbContext;
        _airLabsClient = airLabsClient;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedAirlinesAsync();
        // await SeedAirportsAsync(); // Defer to save API credits during testing if needed
    }

    private async Task SeedAirlinesAsync()
    {
        try
        {
            // 1. Get existing ICAO codes to prevent duplicates
            var existingIcaoCodes = await _dbContext.Airlines
                .Where(a => a.IcaoCode != null)
                .Select(a => a.IcaoCode!)
                .ToListAsync();
            
            var existingSet = new HashSet<string>(existingIcaoCodes);

            // Optimization: If we have plenty of airlines, assume seeded.
            // (Optional: Remove this if you always want to fill gaps)
            if (existingSet.Count > 4000) 
            {
                 _logger.LogInformation("✅ Airlines table robustly populated. Skipping seed.");
                 return;
            }

            _logger.LogInformation("🌱 Seeding Airlines from AirLabs...");
            var airlinesJson = await _airLabsClient.GetAirlinesAsync();

            if (airlinesJson == null) return;

            var newAirlines = new List<Airline>();
            foreach (var el in airlinesJson.Value.EnumerateArray())
            {
                var iata = GetString(el, "iata_code");
                var icao = GetString(el, "icao_code");
                var name = GetString(el, "name");
                var country = GetString(el, "country_code"); 

                // Validation & Truncation
                if (string.IsNullOrEmpty(icao) || existingSet.Contains(icao)) continue; // SKIP EXISTING
                
                if (icao.Length > 3) icao = icao.Substring(0, 3);
                if (!string.IsNullOrEmpty(iata) && iata.Length > 2) iata = iata.Substring(0, 2);
                if (!string.IsNullOrEmpty(country) && country.Length > 2) country = country.Substring(0, 2);

                if (!string.IsNullOrEmpty(name))
                {
                    newAirlines.Add(new Airline
                    {
                         IcaoCode = icao,
                         IataCode = iata,
                         Name = name,
                         CountryCode = country,
                         LogoUrl = !string.IsNullOrEmpty(iata) 
                             ? $"https://daisycon.io/images/airline/?width=300&height=150&color=ffffff&iata={iata}" 
                             : null
                    });
                    
                    // Add to set to handle duplicates within the JSON itself
                    existingSet.Add(icao);
                }
            }

            if (newAirlines.Any())
            {
                await _dbContext.Airlines.AddRangeAsync(newAirlines);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"✅ Added {newAirlines.Count} new airlines.");
            }
            else
            {
                _logger.LogInformation("✅ No new airlines to add.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error seeding Airlines");
        }
    }

    private string? GetString(JsonElement el, string prop) => 
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
}
