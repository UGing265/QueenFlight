using Microsoft.AspNetCore.SignalR;

namespace QueenFlight.API.Hubs;

public class FlightHub : Hub
{
    // Frontend will call this to join a specific "viewport" or just listen globally.
    // For MVP, we broadcast to "All" or a specific "Global" group. 
    // In future, we can map clients to Tile IDs (QuadTree) for optimization.
    public async Task SubscribeToUpdates()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "GlobalFlightData");
    }

    public async Task UnsubscribeFromUpdates()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "GlobalFlightData");
    }
}
