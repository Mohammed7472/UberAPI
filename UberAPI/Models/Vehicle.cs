using System.ComponentModel.DataAnnotations;

namespace UberAPI.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;

    [Required]
    public int DriverId { get; set; }

    public User Driver { get; set; } = null!;
}
