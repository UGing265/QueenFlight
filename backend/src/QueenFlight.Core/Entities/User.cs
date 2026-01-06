using System.ComponentModel.DataAnnotations;

namespace QueenFlight.Core.Entities;

public enum UserRole
{
    Member,
    Admin
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(255)]
    public required string Email { get; set; }

    [StringLength(255)]
    public string? PasswordHash { get; set; }

    public UserRole Role { get; set; } = UserRole.Member;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
