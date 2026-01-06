using Microsoft.EntityFrameworkCore;
using QueenFlight.Core.Entities;

namespace QueenFlight.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Group 1
    public DbSet<Country> Countries { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<AircraftModel> AircraftModels { get; set; }

    // Group 2
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Airport> Airports { get; set; }

    // Group 3
    public DbSet<Aircraft> Aircrafts { get; set; }

    // Group 4
    public DbSet<User> Users { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Group 1 Configurations ---
        modelBuilder.Entity<Country>()
            .Property(c => c.IsoCode)
            .IsFixedLength(); // char(2)

        modelBuilder.Entity<AircraftModel>()
            .HasIndex(m => m.IcaoTypeCode); 

        // --- Group 2 Configurations ---
        modelBuilder.Entity<Airline>()
            .HasIndex(a => a.IcaoCode)
            .IsUnique();

        modelBuilder.Entity<Airport>()
            .HasIndex(a => a.IcaoCode)
            .IsUnique();
        
        // --- Group 3 Configurations ---
        modelBuilder.Entity<Aircraft>()
            .HasIndex(a => a.Icao24)
            .IsUnique(); // Actually PK is already unique, but good to be explicit if it wasn't PK
        
        // --- Group 4 Configurations ---
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>(); // Store as string 'Member', 'Admin'

        // UserPreference 1-to-1 with User
        modelBuilder.Entity<UserPreference>()
            .HasKey(up => up.UserId); // PK is FK

        modelBuilder.Entity<User>()
            .HasOne<UserPreference>()
            .WithOne(up => up.User)
            .HasForeignKey<UserPreference>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
