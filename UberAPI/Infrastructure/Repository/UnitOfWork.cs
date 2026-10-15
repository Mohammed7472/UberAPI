using UberAPI.Data;
using UberAPI.Infrastructure.Contracts;

namespace UberAPI.Infrastructure.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public IAuthRepository Auth { get; }
    public IRideRepository Rides { get; }
    public IVehicleRepository Vehicles { get; }
    public IPaymentRepository Payments { get; }
    public IRatingRepository Ratings { get; }

    public UnitOfWork(
        AppDbContext context,
        IUserRepository users,
        IAuthRepository auth,
        IRideRepository rides,
        IVehicleRepository vehicles,
        IPaymentRepository payments,
        IRatingRepository ratings)
    {
        _context = context;
        Users = users;
        Auth = auth;
        Rides = rides;
        Vehicles = vehicles;
        Payments = payments;
        Ratings = ratings;
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
