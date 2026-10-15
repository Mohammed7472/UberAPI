using Microsoft.EntityFrameworkCore;
using UberAPI.Models;

namespace UberAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Ride> Rides => Set<Ride>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(u => u.Password)
                .HasMaxLength(500);
        });

        // Vehicle configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasIndex(v => v.PlateNumber)
                .IsUnique();

            // Driver (User) 1 : 1 Vehicle
            entity.HasOne(v => v.Driver)
                .WithOne(u => u.Vehicle)
                .HasForeignKey<Vehicle>(v => v.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Ride configuration
        modelBuilder.Entity<Ride>(entity =>
        {
            entity.Property(r => r.Fare)
                .HasPrecision(10, 2);

            entity.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Rider 1 : Many Rides
            entity.HasOne(r => r.Rider)
                .WithMany(u => u.RiderRides)
                .HasForeignKey(r => r.RiderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Driver 1 : Many Rides
            entity.HasOne(r => r.Driver)
                .WithMany(u => u.DriverRides)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => r.RiderId);
            entity.HasIndex(r => r.DriverId);
            entity.HasIndex(r => r.Status);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.Amount)
                .HasPrecision(10, 2);

            entity.Property(p => p.PaymentMethod)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Ride 1 : 1 Payment
            entity.HasOne(p => p.Ride)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RideId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Rating configuration
        modelBuilder.Entity<Rating>(entity =>
        {
            // Ride 1 : 1 Rating
            entity.HasOne(rating => rating.Ride)
                .WithOne(ride => ride.Rating)
                .HasForeignKey<Rating>(rating => rating.RideId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.DriverId)
            .IsUnique();

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.RideId)
            .IsUnique();

        modelBuilder.Entity<Rating>()
            .HasIndex(r => r.RideId)
            .IsUnique();


        // Data Seeding
        modelBuilder.Entity<User>().HasData(
          new User
          {
              Id = 1,
              FullName = "Demo Rider",
              Email = "rider@example.com",
              Password = "123456",
              Phone = "01012345678",
              Role = "Rider"
          },
         new User
         {
             Id = 2,
             FullName = "Demo Driver",
             Email = "driver@example.com",
             Password = "123456",
             Phone = "01087654321",
             Role = "Driver"
         }
);
    }
}
