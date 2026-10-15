using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UberAPI.Data;
using UberAPI.DTOs;
using UberAPI.Infrastructure.Contracts;
using UberAPI.Models;

namespace UberAPI.Infrastructure.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthRepository(IUserRepository userRepository, AppDbContext context, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _context = context;
        _configuration = configuration;
    }

    public async Task<UserDto?> RegisterAsync(User request)
    {
        if (request == null)
            return null;

        var email = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.EmailExistsAsync(email))
            return null;

        request.Email = email;

        await _userRepository.AddAsync(request);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = request.Id,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Role = request.Role
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto request)
    {
        if (request == null)
            return null;

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || request.Password != user.Password)
            return null;

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            Role = user.Role
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("Phone", user.Phone)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


