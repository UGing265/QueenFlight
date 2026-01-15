namespace QueenFlight.Core.DTOs;

public class FlightDetailDto
{
    public string Icao24 { get; set; } = string.Empty;
    public string? Callsign { get; set; }
    public string? Registration { get; set; }
    
    // Airline Info
    public string? AirlineName { get; set; }
    public string? AirlineLogoUrl { get; set; }
    public string? AirlineIata { get; set; }
    
    // Aircraft Info
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? ModelCode { get; set; }
    public string? ImageUrl { get; set; } 
    
    // Flight Info using IATA codes cause IATA are friendly for users
    public string? OriginAirport { get; set; }
    public string? DestinationAirport { get; set; }
        
    public bool IsEnriched { get; set; }
}
