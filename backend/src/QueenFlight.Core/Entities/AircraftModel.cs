using System.ComponentModel.DataAnnotations;

namespace QueenFlight.Core.Entities;

public class AircraftModel
{
    public int Id { get; set; }

    public int ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }

    [StringLength(100)]
    public string? Name { get; set; } // Commercial name

    [StringLength(10)]
    public string? IcaoTypeCode { get; set; } // Critical: B789

    [StringLength(1)]
    public string? WakeTurbulenceCategory { get; set; } // H/M/L
}
