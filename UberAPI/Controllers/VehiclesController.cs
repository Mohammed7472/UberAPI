using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberAPI.DTOs;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public VehiclesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var plateExists = await _unitOfWork.Vehicles.GetByPlateNumberAsync(request.PlateNumber);
        if (plateExists != null)
            return BadRequest(new { message = "Vehicle with this plate number already exists" });

        var vehicle = new Vehicle
        {
            Model = request.Model.Trim(),
            PlateNumber = request.PlateNumber.Trim().ToUpper(),
            Color = request.Color.Trim(),
            DriverId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var driverId) ? driverId : 0
        };

        if (vehicle.DriverId == 0)
            return BadRequest(new { message = "Driver ID not found in token" });

        await _unitOfWork.Vehicles.AddAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            Model = vehicle.Model,
            PlateNumber = vehicle.PlateNumber,
            Color = vehicle.Color,
            DriverId = vehicle.DriverId
        };

        return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.Id }, vehicleDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] UpdateVehicleDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle == null)
            return NotFound(new { message = "Vehicle not found" });

        vehicle.Model = request.Model.Trim();
        vehicle.PlateNumber = request.PlateNumber.Trim().ToUpper();
        vehicle.Color = request.Color.Trim();

        _unitOfWork.Vehicles.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            Model = vehicle.Model,
            PlateNumber = vehicle.PlateNumber,
            Color = vehicle.Color,
            DriverId = vehicle.DriverId
        };

        return Ok(vehicleDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle == null)
            return NotFound(new { message = "Vehicle not found" });

        await _unitOfWork.Vehicles.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVehicleById(int id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle == null)
            return NotFound(new { message = "Vehicle not found" });

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            Model = vehicle.Model,
            PlateNumber = vehicle.PlateNumber,
            Color = vehicle.Color,
            DriverId = vehicle.DriverId
        };

        return Ok(vehicleDto);
    }
}
