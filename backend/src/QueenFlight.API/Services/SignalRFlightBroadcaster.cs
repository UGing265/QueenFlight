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
        await _hubContext.Clients.Group("GlobalFlightData").SendAsync("ReceiveFlightCount", count);
    }

    public async Task BroadcastFlightDataAsync(List<QueenFlight.Core.DTOs.FlightPayloadDto> flights)
    {
        // "ReceiveFlightUpdate" is the method name the Client listens to
        await _hubContext.Clients.Group("GlobalFlightData").SendAsync("ReceiveFlightUpdate", flights);
    }
}
