using Microsoft.EntityFrameworkCore;
using UberAPI.Data;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Infrastructure.Repository;

public class UserRepository
    : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _dbSet.FirstOrDefaultAsync(
            u => u.Email.ToLower() == normalizedEmail);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _dbSet.AnyAsync(
            u => u.Email.ToLower() == normalizedEmail);
    }
}