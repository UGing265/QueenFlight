using QueenFlight.Core.DTOs;

namespace QueenFlight.Core.Interfaces;

public interface IFlightDetailsProvider
{
    Task<FlightDetailDto?> GetFlightDetailsAsync(string icao24);
}
