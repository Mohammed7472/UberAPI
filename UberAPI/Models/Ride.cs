using System.ComponentModel.DataAnnotations;

namespace UberAPI.Models;

public class Ride
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string PickupLocation { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string DropOffLocation { get; set; } = string.Empty;

    [Range(20, 5000)]
    public decimal Fare { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    [Required]
    public int RiderId { get; set; }

    public int? DriverId { get; set; }

    public DateTime RequestedAtUtc { get; set; }
        = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }

    public User Rider { get; set; } = null!;

    public User? Driver { get; set; }

    public Payment? Payment { get; set; }

    public Rating? Rating { get; set; }
}
