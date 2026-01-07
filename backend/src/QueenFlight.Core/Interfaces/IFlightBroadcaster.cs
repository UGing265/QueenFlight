namespace QueenFlight.Core.Interfaces;

public interface IFlightBroadcaster
{
    Task BroadcastFlightCountAsync(int count);
    // Future: Task BroadcastUpdatesAsync(List<FlightState> flights);
}
