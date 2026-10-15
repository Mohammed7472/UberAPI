using Microsoft.EntityFrameworkCore;
using UberAPI.Data;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Infrastructure.Repository;

public class RideRepository : GenericRepository<Ride>, IRideRepository
{
    public RideRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Ride>> GetByRiderIdAsync(int riderId)
    {
        return await _dbSet.Where(r => r.RiderId == riderId).ToListAsync();
    }

    public async Task<List<Ride>> GetByDriverIdAsync(int driverId)
    {
        return await _dbSet.Where(r => r.DriverId == driverId).ToListAsync();
    }

    public async Task<List<Ride>> GetByStatusAsync(string status)
    {
        return await _dbSet.Where(r => r.Status == status).ToListAsync();
    }

    public async Task<List<Ride>> SearchByPickupLocationAsync(string pickupLocation)
    {
        return await _dbSet
            .Where(r => r.PickupLocation.Contains(pickupLocation))
            .ToListAsync();
    }

    public async Task<List<Ride>> FilterByStatusAndFareAsync(string? status, decimal? minFare, decimal? maxFare)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        if (minFare.HasValue)
            query = query.Where(r => r.Fare >= minFare.Value);

        if (maxFare.HasValue)
            query = query.Where(r => r.Fare <= maxFare.Value);

        return await query.ToListAsync();
    }

    public async Task<List<Ride>> GetPaginatedAsync(int pageNumber, int pageSize, string sortBy = "CreatedAt")
    {
        var query = _dbSet.AsQueryable();

        query = sortBy.ToLower() switch
        {
            "fare" => query.OrderBy(r => r.Fare),
            "status" => query.OrderBy(r => r.Status),
            _ => query.OrderByDescending(r => r.RequestedAtUtc)
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
