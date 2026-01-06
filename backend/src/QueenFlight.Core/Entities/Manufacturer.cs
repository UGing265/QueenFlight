using System.ComponentModel.DataAnnotations;

namespace QueenFlight.Core.Entities;

public class Manufacturer
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; } // Boeing, Airbus
}
