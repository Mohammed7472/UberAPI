using UberAPI.Models;

namespace UberAPI.Infrastructure.Contracts;

public interface IUserRepository
    : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);

    Task<bool> EmailExistsAsync(string email);
}