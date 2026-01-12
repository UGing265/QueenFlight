using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("countries")]
public class Country
{
    [Key]
    [StringLength(2)]
    [Column("iso_code")]
    public required string IsoCode { get; set; } // PK, ISO 3166-1 alpha-2

    [StringLength(100)]
    [Column("name")]
    public string? Name { get; set; }

    [StringLength(50)]
    [Column("region")]
    public string? Region { get; set; }
}
