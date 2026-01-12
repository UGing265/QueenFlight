using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("users")]
public class User
{
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [StringLength(255)]
    [Column("email")]
    public required string Email { get; set; }

    [StringLength(255)]
    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("role")]
    public string Role { get; set; } = "member"; // Stored as string, simplified from enum for now or handled in DbContext

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
