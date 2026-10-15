using System.ComponentModel.DataAnnotations;

namespace UberAPI.DTOs;

public class CreateRatingDto
{
    [Required]
    [Range(1, 5)]
    public int Score { get; set; }

    [StringLength(250)]
    public string? Comment { get; set; }

    [Required]
    public int RideId { get; set; }
}

public class RatingDto
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public int RideId { get; set; }
    public DateTime CreatedAt { get; set; }
}
