namespace QueenFlight.Core.Interfaces;

public interface IFlightBroadcaster
{
    Task BroadcastFlightCountAsync(int count);
    Task BroadcastFlightDataAsync(List<QueenFlight.Core.DTOs.FlightPayloadDto> flights);
}
