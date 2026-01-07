using Microsoft.AspNetCore.SignalR;
using QueenFlight.API.Hubs;
using QueenFlight.Core.Interfaces;

namespace QueenFlight.API.Services;

public class SignalRFlightBroadcaster : IFlightBroadcaster
{
    private readonly IHubContext<FlightHub> _hubContext;

    public SignalRFlightBroadcaster(IHubContext<FlightHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastFlightCountAsync(int count)
    {
        await _hubContext.Clients.Group("GlobalFlightData").SendAsync("ReceiveFlightUpdate", count);
    }
}
