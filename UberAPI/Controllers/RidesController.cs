using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberAPI.DTOs;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RidesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RidesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRide([FromBody] CreateRideDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ride = new Ride
        {
            PickupLocation = request.PickupLocation.Trim(),
            DropOffLocation = request.DropOffLocation.Trim(),
            Fare = request.Fare,
            Status = "Pending",
            RiderId = request.RiderId,
            RequestedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Rides.AddAsync(ride);
        await _unitOfWork.SaveChangesAsync();

        var rideDto = new RideDto
        {
            Id = ride.Id,
            PickupLocation = ride.PickupLocation,
            DropOffLocation = ride.DropOffLocation,
            Fare = ride.Fare,
            Status = ride.Status,
            RiderId = ride.RiderId,
            DriverId = ride.DriverId,
            CreatedAt = ride.RequestedAtUtc
        };

        return CreatedAtAction(nameof(GetRideById), new { id = ride.Id }, rideDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetRides([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt")
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(new { message = "Page number and size must be greater than 0" });

        var rides = await _unitOfWork.Rides.GetPaginatedAsync(pageNumber, pageSize, sortBy);

        var rideDtos = rides.Select(r => new RideDto
        {
            Id = r.Id,
            PickupLocation = r.PickupLocation,
            DropOffLocation = r.DropOffLocation,
            Fare = r.Fare,
            Status = r.Status,
            RiderId = r.RiderId,
            DriverId = r.DriverId,
            CreatedAt = r.RequestedAtUtc
        }).ToList();

        var paged = new
        {
            pageNumber,
            pageSize,
            data = rideDtos
        };

        return Ok(paged);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRideById(int id)
    {
        var ride = await _unitOfWork.Rides.GetByIdAsync(id);
        if (ride == null)
            return NotFound(new { message = "Ride not found" });

        var rideDto = new RideDto
        {
            Id = ride.Id,
            PickupLocation = ride.PickupLocation,
            DropOffLocation = ride.DropOffLocation,
            Fare = ride.Fare,
            Status = ride.Status,
            RiderId = ride.RiderId,
            DriverId = ride.DriverId,
            CreatedAt = ride.RequestedAtUtc
        };

        return Ok(rideDto);
    }

    [HttpPatch("{id}/accept")]
    public async Task<IActionResult> AcceptRide(int id)
    {
        var ride = await _unitOfWork.Rides.GetByIdAsync(id);
        if (ride == null)
            return NotFound(new { message = "Ride not found" });

        if (ride.Status != "Pending")
            return BadRequest(new { message = "Ride must be in Pending status to accept" });

        ride.Status = "Accepted";
        ride.DriverId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var driverId) ? driverId : 0;

        _unitOfWork.Rides.Update(ride);
        await _unitOfWork.SaveChangesAsync();

        var rideDto = new RideDto
        {
            Id = ride.Id,
            PickupLocation = ride.PickupLocation,
            DropOffLocation = ride.DropOffLocation,
            Fare = ride.Fare,
            Status = ride.Status,
            RiderId = ride.RiderId,
            DriverId = ride.DriverId,
            CreatedAt = ride.RequestedAtUtc
        };

        return Ok(rideDto);
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteRide(int id)
    {
        var ride = await _unitOfWork.Rides.GetByIdAsync(id);
        if (ride == null)
            return NotFound(new { message = "Ride not found" });

        if (ride.Status != "Accepted")
            return BadRequest(new { message = "Ride must be in Accepted status to complete" });

        ride.Status = "Completed";

        _unitOfWork.Rides.Update(ride);
        await _unitOfWork.SaveChangesAsync();

        var rideDto = new RideDto
        {
            Id = ride.Id,
            PickupLocation = ride.PickupLocation,
            DropOffLocation = ride.DropOffLocation,
            Fare = ride.Fare,
            Status = ride.Status,
            RiderId = ride.RiderId,
            DriverId = ride.DriverId,
            CreatedAt = ride.RequestedAtUtc
        };

        return Ok(rideDto);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRide(int id)
    {
        var ride = await _unitOfWork.Rides.GetByIdAsync(id);
        if (ride == null)
            return NotFound(new { message = "Ride not found" });

        await _unitOfWork.Rides.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

  
    [HttpGet("search")]
    public async Task<IActionResult> SearchRides([FromQuery] string pickup)
    {
        if (string.IsNullOrWhiteSpace(pickup))
            return BadRequest(new { message = "Pickup location is required" });

        var rides = await _unitOfWork.Rides.SearchByPickupLocationAsync(pickup);

        var rideDtos = rides.Select(r => new RideDto
        {
            Id = r.Id,
            PickupLocation = r.PickupLocation,
            DropOffLocation = r.DropOffLocation,
            Fare = r.Fare,
            Status = r.Status,
            RiderId = r.RiderId,
            DriverId = r.DriverId,
            CreatedAt = r.RequestedAtUtc
        }).ToList();

        return Ok(rideDtos);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> FilterRides([FromQuery] string? status, [FromQuery] decimal? minFare, [FromQuery] decimal? maxFare, [FromQuery] string sortBy = "CreatedAt")
    {
        var rides = await _unitOfWork.Rides.FilterByStatusAndFareAsync(status, minFare, maxFare);

        rides = sortBy.ToLower() switch
        {
            "fare" => rides.OrderBy(r => r.Fare).ToList(),
            "status" => rides.OrderBy(r => r.Status).ToList(),
            _ => rides.OrderByDescending(r => r.RequestedAtUtc).ToList()
        };

        var rideDtos = rides.Select(r => new RideDto
        {
            Id = r.Id,
            PickupLocation = r.PickupLocation,
            DropOffLocation = r.DropOffLocation,
            Fare = r.Fare,
            Status = r.Status,
            RiderId = r.RiderId,
            DriverId = r.DriverId,
            CreatedAt = r.RequestedAtUtc
        }).ToList();

        return Ok(rideDtos);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetRidesReport()
    {
        var rides = await _unitOfWork.Rides.GetAllAsync();

        var report = new
        {
            TotalRides = rides.Count(),
            PendingRides = rides.Count(r => r.Status == "Pending"),
            AcceptedRides = rides.Count(r => r.Status == "Accepted"),
            CompletedRides = rides.Count(r => r.Status == "Completed"),
            TotalFare = rides.Sum(r => r.Fare),
            AverageFare = rides.Count > 0 ? rides.Average(r => r.Fare) : 0
        };

        return Ok(report);
    }
}
