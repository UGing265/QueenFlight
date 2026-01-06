using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

public class Aircraft
{
    [Key]
    [StringLength(24)]
    public required string Icao24 { get; set; } // Unique Hex Code

    [StringLength(20)]
    public string? RegistrationNumber { get; set; } // VN-A868

    public int? ModelId { get; set; }
    public AircraftModel? Model { get; set; }

    public int? AirlineId { get; set; }
    public Airline? Airline { get; set; }

    [StringLength(100)]
    public string? Owner { get; set; }

    public DateTime LastMetadataUpdate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
