using System.ComponentModel.DataAnnotations;

namespace UberAPI.Models;

public class Payment
{
    public int Id { get; set; }

    [Range(typeof(decimal), "20", "5000")]
    public decimal Amount { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    public DateTime PaymentDateUtc { get; set; }
        = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "Pending";

    [Required]
    public int RideId { get; set; }

    public Ride Ride { get; set; } = null!;
}
