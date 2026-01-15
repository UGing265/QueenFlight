using Microsoft.AspNetCore.Mvc;
using QueenFlight.Core.DTOs;
using QueenFlight.Core.Interfaces;

namespace QueenFlight.API.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightDetailsController : ControllerBase
{
    private readonly IFlightDetailsProvider _flightDetailsProvider;
    private readonly ILogger<FlightDetailsController> _logger;

    public FlightDetailsController(IFlightDetailsProvider flightDetailsProvider, ILogger<FlightDetailsController> logger)
    {
        _flightDetailsProvider = flightDetailsProvider;
        _logger = logger;
    }

    [HttpGet("{icao24}")]
    public async Task<ActionResult<FlightDetailDto>> GetFlightDetails(string icao24)
    {
        if (string.IsNullOrWhiteSpace(icao24))
        {
            return BadRequest("ICAO24 code is required.");
        }

        try
        {
            var details = await _flightDetailsProvider.GetFlightDetailsAsync(icao24);
            
            if (details == null)
            {
                return NotFound($"No details found for aircraft {icao24}");
            }

            return Ok(details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching details for {Icao24}", icao24);
            // Return 500 but considering masking the error in prod
            return StatusCode(500, "Internal server error while fetching flight details.");
        }
    }
}
