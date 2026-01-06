using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

public class Airport
{
    [Key]
    [StringLength(3)]
    public required string IataCode { get; set; } // SGN

    [StringLength(4)]
    public string? IcaoCode { get; set; } // VVTS

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }
    [ForeignKey("CountryCode")]
    public Country? Country { get; set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(9,6)")]
    public decimal? Longitude { get; set; }

    [StringLength(50)]
    public string? Timezone { get; set; }
}
