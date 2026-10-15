using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberAPI.DTOs;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RatingsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RatingsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ride = await _unitOfWork.Rides.GetByIdAsync(request.RideId);
        if (ride == null)
            return NotFound(new { message = "Ride not found" });

        var existingRating = await _unitOfWork.Ratings.GetByRideIdAsync(request.RideId);
        if (existingRating != null)
            return BadRequest(new { message = "Rating already exists for this ride" });

        var rating = new Rating
        {
            Score = request.Score,
            Comment = request.Comment,
            RideId = request.RideId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Ratings.AddAsync(rating);
        await _unitOfWork.SaveChangesAsync();

        var ratingDto = new RatingDto
        {
            Id = rating.Id,
            Score = rating.Score,
            Comment = rating.Comment,
            RideId = rating.RideId,
            CreatedAt = rating.CreatedAtUtc
        };

        return CreatedAtAction(nameof(GetRatingById), new { id = rating.Id }, ratingDto);
    }

    [HttpGet("top-drivers")]
    public async Task<IActionResult> GetTopDrivers([FromQuery] int count = 10)
    {
        if (count < 1 || count > 100)
            return BadRequest(new { message = "Count must be between 1 and 100" });

        var topDrivers = await _unitOfWork.Ratings.GetTopDriversAsync(count);

        var topDriverDtos = topDrivers.Select(d => new UserDto
        {
            Id = d.Id,
            FullName = d.FullName,
            Email = d.Email,
            Phone = d.Phone,
            Role = d.Role
        }).ToList();

        return Ok(topDriverDtos);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetRatingById(int id)
    {
        var rating = await _unitOfWork.Ratings.GetByIdAsync(id);
        if (rating == null)
            return NotFound(new { message = "Rating not found" });

        var ratingDto = new RatingDto
        {
            Id = rating.Id,
            Score = rating.Score,
            Comment = rating.Comment,
            RideId = rating.RideId,
            CreatedAt = rating.CreatedAtUtc
        };

        return Ok(ratingDto);
    }
}
