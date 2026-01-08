using MessagePack;

namespace QueenFlight.Core.DTOs;

[MessagePackObject]
public class FlightPayloadDto
{
    [Key("i")] 
    public string Icao24 { get; set; } = string.Empty;

    [Key("la")]
    public double Lat { get; set; }

    [Key("lo")]
    public double Lng { get; set; }

    [Key("v")]
    public float Velocity { get; set; } // m/s or km/h depending on source, using float for size

    [Key("h")]
    public float Heading { get; set; }

    [Key("ts")]
    public long ServerTimestamp { get; set; }
}
