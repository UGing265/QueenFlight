using QueenFlight.Core.Models;

namespace QueenFlight.Infrastructure.Interfaces;

public interface IFlightCache
{
    Task SetFlightAsync(FlightState flight);
    Task<FlightState?> GetFlightAsync(string icao24);
    Task<List<FlightState>> GetAllFlightsAsync();
}
