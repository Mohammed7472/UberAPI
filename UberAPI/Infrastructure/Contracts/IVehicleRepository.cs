using UberAPI.Models;

namespace UberAPI.Infrastructure.Contracts;

public interface IVehicleRepository : IGenericRepository<Vehicle>
{
    Task<Vehicle?> GetByDriverIdAsync(int driverId);
    Task<Vehicle?> GetByPlateNumberAsync(string plateNumber);
}
