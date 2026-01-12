using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("aircraft_models")]
public class AircraftModel
{
    [Column("id")]
    public int Id { get; set; }

    [Column("manufacturer_id")]
    public int ManufacturerId { get; set; }
    
    [ForeignKey("ManufacturerId")]
    public Manufacturer? Manufacturer { get; set; }

    [StringLength(100)]
    [Column("name")]
    public string? Name { get; set; }

    [StringLength(10)]
    [Column("icao_type_code")]
    public string? IcaoTypeCode { get; set; } // Critical for mapping

    [StringLength(1)]
    [Column("wake_turbulence_category")]
    public string? WakeTurbulenceCategory { get; set; } // H/M/L
}
