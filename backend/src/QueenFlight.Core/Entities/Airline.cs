using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("airlines")]
public class Airline
{
    [Column("id")]
    public int Id { get; set; }

    [StringLength(3)]
    [Column("icao_code")]
    public string? IcaoCode { get; set; } // HVN

    [StringLength(2)]
    [Column("iata_code")]
    public string? IataCode { get; set; } // VN

    [StringLength(100)]
    [Column("name")]
    public string? Name { get; set; }

    [StringLength(2)]
    [Column("country_code")]
    public string? CountryCode { get; set; }
    
    [ForeignKey("CountryCode")]
    public Country? Country { get; set; }

    [StringLength(50)]
    [Column("callsign_prefix")]
    public string? CallsignPrefix { get; set; } 

    [StringLength(255)]
    [Column("logo_url")]
    public string? LogoUrl { get; set; }
}
