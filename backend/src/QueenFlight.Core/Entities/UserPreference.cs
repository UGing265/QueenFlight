using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

public class UserPreference
{
    [Key, ForeignKey("User")]
    public Guid UserId { get; set; }
    public User? User { get; set; }

    [StringLength(50)]
    public string DefaultMapStyle { get; set; } = "dark";

    [StringLength(3)]
    public string? HomeAirportIata { get; set; }
    [ForeignKey("HomeAirportIata")]
    public Airport? HomeAirport { get; set; }

    public bool ShowWeather { get; set; } = false;
}
