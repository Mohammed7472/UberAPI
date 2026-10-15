using System.ComponentModel.DataAnnotations;

namespace UberAPI.DTOs;

public class CreateVehicleDto
{
    [Required]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Color { get; set; } = string.Empty;
}

public class UpdateVehicleDto
{
    [Required]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Color { get; set; } = string.Empty;
}

public class VehicleDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int DriverId { get; set; }
}
