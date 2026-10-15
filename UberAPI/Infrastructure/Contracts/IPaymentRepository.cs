using UberAPI.Models;

namespace UberAPI.Infrastructure.Contracts;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByRideIdAsync(int rideId);
    Task<List<Payment>> GetByStatusAsync(string status);
    Task<decimal> GetTotalAmountAsync();
    Task<int> GetCountByStatusAsync(string status);
}
