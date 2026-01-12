using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("airports")]
public class Airport
{
    [Key]
    [StringLength(15)]
    [Column("ident")]
    public required string Ident { get; set; }

    [StringLength(3)]
    [Column("iata_code")]
    public string? IataCode { get; set; }

    [StringLength(4)]
    [Column("icao_code")]
    public string? IcaoCode { get; set; }

    [StringLength(100)]
    [Column("name")]
    public string? Name { get; set; }

    [StringLength(100)]
    [Column("city")]
    public string? City { get; set; }

    [StringLength(2)]
    [Column("country_code")]
    public string? CountryCode { get; set; }
    
    [ForeignKey("CountryCode")]
    public Country? Country { get; set; }

    [Column("latitude", TypeName = "decimal(9,6)")]
    public decimal? Latitude { get; set; }

    [Column("longitude", TypeName = "decimal(9,6)")]
    public decimal? Longitude { get; set; }
}
