using System.ComponentModel.DataAnnotations;

namespace UberAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Rider";

    public Vehicle? Vehicle { get; set; }

    public ICollection<Ride> RiderRides { get; set; }
        = new List<Ride>();

    public ICollection<Ride> DriverRides { get; set; }
        = new List<Ride>();
}
