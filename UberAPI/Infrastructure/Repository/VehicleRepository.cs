using Microsoft.EntityFrameworkCore;
using UberAPI.Data;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Infrastructure.Repository;

public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Vehicle?> GetByDriverIdAsync(int driverId)
    {
        return await _dbSet.FirstOrDefaultAsync(v => v.DriverId == driverId);
    }

    public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);
    }
}
