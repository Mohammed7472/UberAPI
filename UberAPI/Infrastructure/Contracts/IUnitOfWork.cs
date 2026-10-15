namespace UberAPI.Infrastructure.Contracts;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IAuthRepository Auth { get; }
    IRideRepository Rides { get; }
    IVehicleRepository Vehicles { get; }
    IPaymentRepository Payments { get; }
    IRatingRepository Ratings { get; }

    Task<int> SaveChangesAsync();
}
