using DentalBooking.AuthService.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalBooking.AuthService.Infrastructure.Jwt;

public interface IJwtProvider
{
    string GenerateToken(ApplicationUser user);
}

public class JwtProvider(IConfiguration config) : IJwtProvider
{
    private readonly IConfiguration _config = config;

    public string GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id ?? Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Role, user.Role ?? ""),
            new(ClaimTypes.GivenName, user.FirstName ?? ""),
            new(ClaimTypes.Surname, user.LastName ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
