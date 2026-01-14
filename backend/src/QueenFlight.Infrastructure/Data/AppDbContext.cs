using Microsoft.EntityFrameworkCore;
using QueenFlight.Core.Entities;

namespace QueenFlight.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Group 1: Reference Data
    public DbSet<Country> Countries { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<AircraftModel> AircraftModels { get; set; }

    // Group 2: Metadata
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Airport> Airports { get; set; }

    // Group 3: Registry
    public DbSet<Aircraft> Aircrafts { get; set; }

    // Group 4: Users
    public DbSet<User> Users { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Group 1: Reference Data ---
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IsoCode);
            entity.Property(e => e.IsoCode).IsFixedLength();
        });

        modelBuilder.Entity<AircraftModel>(entity =>
        {
            entity.HasOne(d => d.Manufacturer)
                .WithMany()
                .HasForeignKey(d => d.ManufacturerId);
            
            // Index for mapping logic (not explicitly unique in schema but good for performance)
            entity.HasIndex(e => e.IcaoTypeCode);
        });

        // --- Group 2: Flight Metadata ---
        modelBuilder.Entity<Airline>(entity =>
        {
            entity.HasIndex(e => e.IcaoCode).IsUnique();
            entity.Property(e => e.IataCode).IsFixedLength();
            
            entity.HasOne(d => d.Country)
                .WithMany()
                .HasForeignKey(d => d.CountryCode);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Ident);
            entity.HasIndex(e => e.IcaoCode).IsUnique();
            entity.HasIndex(e => e.IataCode).IsUnique();

            entity.HasOne(d => d.Country)
                .WithMany()
                .HasForeignKey(d => d.CountryCode);
        });

        // --- Group 3: Aircraft Registry ---
        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Icao24);
            
            entity.HasOne(d => d.Model)
                .WithMany()
                .HasForeignKey(d => d.ModelId);

            entity.HasOne(d => d.Airline)
                .WithMany()
                .HasForeignKey(d => d.AirlineId);
            
            // Default values handled in entity property initializers or here
            entity.Property(e => e.LastMetadataUpdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Postgres syntax, assuming Npgsql
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // --- Group 4: Users & Settings ---
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Role).HasDefaultValue("member");
            
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<UserPreference>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.DefaultMapStyle).HasDefaultValue("dark");
            entity.Property(e => e.ShowWeather).HasDefaultValue(false);

            entity.HasOne(d => d.User)
                .WithOne() // 1-to-1
                .HasForeignKey<UserPreference>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(d => d.HomeAirport)
                .WithMany()
                .HasForeignKey(d => d.HomeAirportIata)
                .HasPrincipalKey(a => a.IataCode); // Point to IataCode, not Ident

        });
    }
}
