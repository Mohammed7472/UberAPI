using Microsoft.EntityFrameworkCore;
using UberAPI.Data;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Infrastructure.Repository;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByRideIdAsync(int rideId)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.RideId == rideId);
    }

    public async Task<List<Payment>> GetByStatusAsync(string status)
    {
        return await _dbSet.Where(p => p.Status == status).ToListAsync();
    }

    public async Task<decimal> GetTotalAmountAsync()
    {
        return await _dbSet.SumAsync(p => p.Amount);
    }

    public async Task<int> GetCountByStatusAsync(string status)
    {
        return await _dbSet.CountAsync(p => p.Status == status);
    }
}
