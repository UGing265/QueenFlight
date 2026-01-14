using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueenFlight.Core.Entities;

[Table("user_preferences")]
public class UserPreference
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }

    [StringLength(50)]
    [Column("default_map_style")]
    public string DefaultMapStyle { get; set; } = "dark";

    [StringLength(3)]
    [Column("home_airport_iata")]
    public string? HomeAirportIata { get; set; }
    
    [ForeignKey("HomeAirportIata")]
    public Airport? HomeAirport { get; set; }

    [Column("show_weather")]
    public bool ShowWeather { get; set; } = false;
}
