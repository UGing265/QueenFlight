using Microsoft.AspNetCore.SignalR;

namespace QueenFlight.API.Hubs;

public class FlightHub : Hub
{
    private readonly ILogger<FlightHub> _logger;

    public FlightHub(ILogger<FlightHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"🔌 Client Connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogWarning($"🔌 Client Disconnected: {Context.ConnectionId}. Error: {exception?.Message}");
        await base.OnDisconnectedAsync(exception);
    }

    // Frontend will call this to join a specific "viewport" or just listen globally.
    // For MVP, we broadcast to "All" or a specific "Global" group. 
    // In future, we can map clients to Tile IDs (QuadTree) for optimization.
    public async Task SubscribeToUpdates()
    {
        _logger.LogInformation($"🔔 Client {Context.ConnectionId} subscribed to GlobalFlightData");
        await Groups.AddToGroupAsync(Context.ConnectionId, "GlobalFlightData");
    }

    public async Task UnsubscribeFromUpdates()
    {
        _logger.LogInformation($"🔕 Client {Context.ConnectionId} unsubscribed");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "GlobalFlightData");
    }
}
