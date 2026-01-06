using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

public class Airline
{
    public int Id { get; set; }

    [StringLength(3)]
    public string? IcaoCode { get; set; } // HVN

    [StringLength(2)]
    public string? IataCode { get; set; } // VN

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }
    [ForeignKey("CountryCode")]
    public Country? Country { get; set; }

    [StringLength(50)]
    public string? CallsignPrefix { get; set; } // VIET NAM

    [StringLength(255)]
    public string? LogoUrl { get; set; }
}
