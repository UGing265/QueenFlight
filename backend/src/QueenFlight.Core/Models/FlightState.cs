namespace QueenFlight.Core.Models;

/// <summary>
/// Represents real-time flight state derived from OpenSky Network API.
/// See: https://openskynetwork.github.io/opensky-api/rest.html#response
/// </summary>
public record FlightState(
    string Icao24,          // [0] Unique ICAO 24-bit address of the transponder in hex string representation.
    string? Callsign,        // [1] Callsign of the vehicle (8 chars). Can be null if no callsign has been received.
    string? OriginCountry,   // [2] Country name inferred from the ICAO 24-bit address.
    float? Longitude,       // [5] WGS-84 longitude in decimal degrees. Can be null.
    float? Latitude,        // [6] WGS-84 latitude in decimal degrees. Can be null.
    float? BaroAltitude,    // [7] Barometric altitude in meters. Can be null.
    bool OnGround,          // [8] Boolean value which indicates if the position was retrieved from a surface position report.
    float? Velocity,        // [9] Velocity over ground in m/s. Can be null.
    float? TrueTrack,       // [10] True track in decimal degrees clockwise from north (north=0°). Can be null.
    float? VerticalRate,    // [11] Vertical rate in m/s. A positive value indicates that the airplane is climbing. Can be null.
    // Sensors
    int? GeoAltitude,       // [13] Geometric altitude in meters. Can be null.
    string? Squawk,          // [14] The transponder code aka Squawk. Can be null.
    bool Spi,               // [15] Whether flight status indicates special purpose indicator.
    int PositionSource      // [16] Origin of this state's position: 0 = ADS-B, 1 = ASTERIX, 2 = MLAT
)
{
    // Record creates immutable data structure perfect for high-frequency updates
    public DateTime LastUpdate { get; init; } = DateTime.UtcNow;
}
