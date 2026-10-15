using UberAPI.Models;

namespace UberAPI.Infrastructure.Contracts;

public interface IRatingRepository : IGenericRepository<Rating>
{
    Task<Rating?> GetByRideIdAsync(int rideId);
    Task<List<Rating>> GetByScoreAsync(int score);
    Task<double> GetAverageScoreAsync();
    Task<List<User>> GetTopDriversAsync(int count = 10);
}
