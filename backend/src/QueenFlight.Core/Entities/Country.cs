using System.ComponentModel.DataAnnotations;

namespace QueenFlight.Core.Entities;

public class Country
{
    [Key]
    [StringLength(2)]
    public required string IsoCode { get; set; } // PK, ISO 3166-1 alpha-2

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Region { get; set; }
}
