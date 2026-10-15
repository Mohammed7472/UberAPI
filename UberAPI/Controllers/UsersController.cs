using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberAPI.DTOs;
using UberAPI.Infrastructure.Contracts;

namespace UberAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("drivers")]
    [Authorize]
    public async Task<IActionResult> GetDrivers()
    {
        var drivers = await _unitOfWork.Users.GetAllAsync();
        var driverList = drivers.Where(u => u.Role == "Driver").Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role
        }).ToList();

        return Ok(driverList);
    }

    [HttpGet("riders/search")]
    [Authorize]
    public async Task<IActionResult> SearchRiders([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Name parameter is required" });

        var riders = await _unitOfWork.Users.GetAllAsync();
        var riderList = riders
            .Where(u => u.Role == "Rider" && u.FullName.Contains(name, StringComparison.OrdinalIgnoreCase))
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role
            })
            .ToList();

        return Ok(riderList);
    }
}
