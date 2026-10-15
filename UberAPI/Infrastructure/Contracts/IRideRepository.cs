using UberAPI.Models;

namespace UberAPI.Infrastructure.Contracts;

public interface IRideRepository : IGenericRepository<Ride>
{
    Task<List<Ride>> GetByRiderIdAsync(int riderId);
    Task<List<Ride>> GetByDriverIdAsync(int driverId);
    Task<List<Ride>> GetByStatusAsync(string status);
    Task<List<Ride>> SearchByPickupLocationAsync(string pickupLocation);
    Task<List<Ride>> FilterByStatusAndFareAsync(string? status, decimal? minFare, decimal? maxFare);
    Task<List<Ride>> GetPaginatedAsync(int pageNumber, int pageSize, string sortBy = "CreatedAt");
}
