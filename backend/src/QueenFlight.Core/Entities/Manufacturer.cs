using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("manufacturers")]
public class Manufacturer
{
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("name")]
    public required string Name { get; set; }
}
