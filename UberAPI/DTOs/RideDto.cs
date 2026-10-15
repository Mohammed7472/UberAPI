using System.ComponentModel.DataAnnotations;

namespace UberAPI.DTOs;

public class CreateRideDto
{
    [Required]
    [StringLength(150)]
    public string PickupLocation { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string DropOffLocation { get; set; } = string.Empty;

    [Required]
    [Range(20, 5000)]
    public decimal Fare { get; set; }

    [Required]
    public int RiderId { get; set; }
}

public class RideDto
{
    public int Id { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string DropOffLocation { get; set; } = string.Empty;
    public decimal Fare { get; set; }
    public string Status { get; set; } = "Pending";
    public int? RiderId { get; set; }
    public int? DriverId { get; set; }
    public DateTime CreatedAt { get; set; }
}
