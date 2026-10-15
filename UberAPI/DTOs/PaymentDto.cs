using System.ComponentModel.DataAnnotations;

namespace UberAPI.DTOs;

public class CreatePaymentDto
{
    [Required]
    [Range(20, 5000)]
    public decimal Amount { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    public int RideId { get; set; }
}

public class PaymentDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = "Pending";
    public int RideId { get; set; }
}
