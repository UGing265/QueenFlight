using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("aircrafts")]
public class Aircraft
{
    [Key]
    [StringLength(24)]
    [Column("icao24")]
    public required string Icao24 { get; set; } // Unique Hex Code

    [StringLength(20)]
    [Column("registration_number")]
    public string? RegistrationNumber { get; set; } 

    [Column("model_id")]
    public int? ModelId { get; set; }
    
    [ForeignKey("ModelId")]
    public AircraftModel? Model { get; set; }

    [Column("airline_id")]
    public int? AirlineId { get; set; }
    
    [ForeignKey("AirlineId")]
    public Airline? Airline { get; set; }

    [StringLength(100)]
    [Column("owner")]
    public string? Owner { get; set; }

    [Column("last_metadata_update")]
    public DateTime LastMetadataUpdate { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
