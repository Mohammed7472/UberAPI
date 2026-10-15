using UberAPI.DTOs;
using UberAPI.Models;

namespace UberAPI.Infrastructure.Contracts;

public interface IAuthRepository
{
    Task<UserDto?> RegisterAsync(User request);

    Task<AuthResponseDto?> LoginAsync(LoginDto request);
}
