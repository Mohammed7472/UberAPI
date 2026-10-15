using Microsoft.EntityFrameworkCore;
using UberAPI.Data;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Infrastructure.Repository;

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    private readonly AppDbContext _context;

    public RatingRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Rating?> GetByRideIdAsync(int rideId)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.RideId == rideId);
    }

    public async Task<List<Rating>> GetByScoreAsync(int score)
    {
        return await _dbSet.Where(r => r.Score == score).ToListAsync();
    }

    public async Task<double> GetAverageScoreAsync()
    {
        var count = await _dbSet.CountAsync();
        if (count == 0) return 0;

        var totalScore = await _dbSet.SumAsync(r => r.Score);
        return (double)totalScore / count;
    }

    public async Task<List<User>> GetTopDriversAsync(int count = 10)
    {
        return await _context.Ratings
            .Include(r => r.Ride)
            .ThenInclude(ride => ride.Driver)
            .Where(r => r.Ride != null && r.Ride.Driver != null)
            .GroupBy(r => r.Ride.DriverId)
            .OrderByDescending(g => g.Average(r => r.Score))
            .Take(count)
            .Select(g => g.First().Ride.Driver)
            .ToListAsync();
    }
}
