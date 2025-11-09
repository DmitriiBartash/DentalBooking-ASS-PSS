using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalBooking.Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration config) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IConfiguration _config = config;

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return null;

        if (!user.EmailConfirmed)
            return null;

        if (!await _userManager.CheckPasswordAsync(user, password))
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
