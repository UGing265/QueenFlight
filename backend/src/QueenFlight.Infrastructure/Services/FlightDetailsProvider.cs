using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueenFlight.Core.DTOs;
using QueenFlight.Core.Entities;
using QueenFlight.Core.Interfaces;
using QueenFlight.Infrastructure.Data;
using QueenFlight.Infrastructure.ExternalServices;

namespace QueenFlight.Infrastructure.Services;

public class FlightDetailsProvider : IFlightDetailsProvider
{
    private readonly AppDbContext _dbContext;
    private readonly IAirLabsClient _airLabsClient;
    private readonly ILogger<FlightDetailsProvider> _logger;

    public FlightDetailsProvider(
        AppDbContext dbContext,
        IAirLabsClient airLabsClient,
        ILogger<FlightDetailsProvider> logger)
    {
        _dbContext = dbContext;
        _airLabsClient = airLabsClient;
        _logger = logger;
    }

    public async Task<FlightDetailDto?> GetFlightDetailsAsync(string icao24)
    {
        // 1. Check local DB (Lazy Loading)
        var aircraft = await _dbContext.Aircrafts
            .Include(a => a.Airline)
            .Include(a => a.Model)
            .ThenInclude(m => m!.Manufacturer)
            .FirstOrDefaultAsync(a => a.Icao24 == icao24);

        if (aircraft != null && aircraft.Airline != null)
        {
            _logger.LogInformation($"✅ CACHE HIT: Found metadata for {icao24} in DB.");
            return MapToDto(aircraft);
        }

        // 2. Not found or incomplete? Fetch from AirLabs
        _logger.LogInformation($"⚠️ CACHE MISS: Fetching metadata for {icao24} from AirLabs...");
        var flightJson = await _airLabsClient.GetFlightDetailsAsync(icao24);

        if (flightJson == null)
        {
             // Fallback: Return what we have (even if empty) or null
             return aircraft != null ? MapToDto(aircraft) : null;
        }

        // 3. Auto-Seed Logic (The Magic Step)
        var enrichedAircraft = await AutoSeedMetadataAsync(icao24, flightJson.Value, aircraft);

        return MapToDto(enrichedAircraft);
    }

    private async Task<Aircraft> AutoSeedMetadataAsync(string icao24, JsonElement json, Aircraft? existingAircraft)
    {
        // Extract Data
        string? airlineIata = GetString(json, "airline_iata");
        string? airlineIcao = GetString(json, "airline_icao");
        string? airlineName = GetString(json, "airline_name");
        
        string? modelCode = GetString(json, "model_code") ?? GetString(json, "aircraft_icao"); // B789
        string? modelName = GetString(json, "model") ?? GetString(json, "aircraft_name"); // Boeing 787-9 Dreamliner
        string? manufacturerName = GetString(json, "manufacturer");
        string? regNumber = GetString(json, "reg_number");

        // --- Transaction Scope for Integrity ---
        // Note: EF Core tracks changes, so we just attach or add
        
        // A. Handle Airline
        Airline? airline = null;
        if (!string.IsNullOrEmpty(airlineIcao))
        {
            airline = await _dbContext.Airlines.FirstOrDefaultAsync(a => a.IcaoCode == airlineIcao);
            if (airline == null)
            {
                airline = new Airline
                {
                    IcaoCode = airlineIcao,
                    IataCode = airlineIata,
                    Name = airlineName ?? "Unknown Airline",
                    LogoUrl = !string.IsNullOrEmpty(airlineIata) 
                        ? $"https://daisycon.io/images/airline/?width=300&height=150&color=ffffff&iata={airlineIata}" 
                        : null
                };
                _dbContext.Airlines.Add(airline);
                await _dbContext.SaveChangesAsync(); // Commit immediately to get ID
                _logger.LogInformation($"🌱 SEEDED Airline: {airlineName} ({airlineIcao})");
            }
        }

        // B. Handle Manufacturer & Model
        AircraftModel? model = null;
        if (!string.IsNullOrEmpty(modelCode))
        {
            model = await _dbContext.AircraftModels
                .Include(m => m.Manufacturer)
                .FirstOrDefaultAsync(m => m.IcaoTypeCode == modelCode);

            if (model == null)
            {
                // Ensure Manufacturer exists
                var mfrName = manufacturerName ?? "Unknown Manufacturer";
                var mfr = await _dbContext.Manufacturers.FirstOrDefaultAsync(m => m.Name == mfrName);
                if (mfr == null)
                {
                    mfr = new Manufacturer { Name = mfrName };
                    _dbContext.Manufacturers.Add(mfr);
                    await _dbContext.SaveChangesAsync();
                }

                model = new AircraftModel
                {
                    IcaoTypeCode = modelCode,
                    Name = modelName ?? modelCode,
                    ManufacturerId = mfr.Id
                };
                _dbContext.AircraftModels.Add(model);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"🌱 SEEDED Model: {modelName} ({modelCode})");
            }
        }

        // C. Update/Insert Aircraft
        if (existingAircraft == null)
        {
            existingAircraft = new Aircraft { Icao24 = icao24 };
            _dbContext.Aircrafts.Add(existingAircraft);
        }

        existingAircraft.RegistrationNumber = regNumber;
        if (airline != null) existingAircraft.AirlineId = airline.Id;
        if (model != null) existingAircraft.ModelId = model.Id;
        existingAircraft.LastMetadataUpdate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return existingAircraft;
    }

    private FlightDetailDto MapToDto(Aircraft aircraft)
    {
        return new FlightDetailDto
        {
            Icao24 = aircraft.Icao24,
            Registration = aircraft.RegistrationNumber,
            AirlineName = aircraft.Airline?.Name,
            AirlineIata = aircraft.Airline?.IataCode,
            AirlineLogoUrl = aircraft.Airline?.LogoUrl,
            Manufacturer = aircraft.Model?.Manufacturer?.Name,
            Model = aircraft.Model?.Name,
            ModelCode = aircraft.Model?.IcaoTypeCode,
            IsEnriched = aircraft.AirlineId.HasValue
        };
    }

    private string? GetString(JsonElement el, string prop) => 
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
}
