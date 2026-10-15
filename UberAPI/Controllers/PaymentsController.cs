using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UberAPI.DTOs;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ride = await _unitOfWork.Rides.GetByIdAsync(request.RideId);
        if (ride == null)
            return NotFound(new { message = "Ride not found" });

        var existingPayment = await _unitOfWork.Payments.GetByRideIdAsync(request.RideId);
        if (existingPayment != null)
            return BadRequest(new { message = "Payment already exists for this ride" });

        var payment = new Payment
        {
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod.Trim(),
            PaymentDateUtc = DateTime.UtcNow,
            Status = "Completed",
            RideId = request.RideId
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        var paymentDto = new PaymentDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentDate = payment.PaymentDateUtc,
            Status = payment.Status,
            RideId = payment.RideId
        };

        return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, paymentDto);
    }

 
    [HttpGet("statistics")]
    public async Task<IActionResult> GetPaymentStatistics()
    {
        var totalAmount = await _unitOfWork.Payments.GetTotalAmountAsync();
        var completedCount = await _unitOfWork.Payments.GetCountByStatusAsync("Completed");
        var pendingCount = await _unitOfWork.Payments.GetCountByStatusAsync("Pending");

        var statistics = new
        {
            TotalAmount = totalAmount,
            CompletedPayments = completedCount,
            PendingPayments = pendingCount,
            AverageAmount = completedCount > 0 ? totalAmount / completedCount : 0
        };

        return Ok(statistics);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        var paymentDto = new PaymentDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentDate = payment.PaymentDateUtc,
            Status = payment.Status,
            RideId = payment.RideId
        };

        return Ok(paymentDto);
    }
}
