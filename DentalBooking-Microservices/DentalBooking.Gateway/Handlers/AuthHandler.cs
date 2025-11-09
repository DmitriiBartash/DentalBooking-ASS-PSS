using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DentalBooking.Gateway.Handlers;

public class AuthHandler(IConfiguration config) : GatewayHandler
{
    private readonly IConfiguration _config = config;

    protected override async Task<bool> ProcessAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (path != null && (path.StartsWith("/api/auth/login") || path.StartsWith("/api/auth/register")))
        {
            Console.WriteLine($"[AuthHandler] Skipping token validation for: {path}");
            return false;
        }

        Console.WriteLine("[AuthHandler] Checking Authorization header...");

        if (!context.Request.Headers.TryGetValue("Authorization", out var token))
        {
            Console.WriteLine("[AuthHandler] Authorization header missing");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: No token provided");
            return true;
        }

        var tokenString = token.ToString().Replace("Bearer ", "");
        Console.WriteLine($"[AuthHandler] Token received: {tokenString[..Math.Min(30, tokenString.Length)]}...");

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config["Jwt:Issuer"],
            ValidAudience = _config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(tokenString, validationParams, out var validatedToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;

            context.Items["UserEmail"] = email;
            context.Items["UserRole"] = role;

            Console.WriteLine("[AuthHandler] Token validation succeeded");
            Console.WriteLine($"[AuthHandler] User: {email}, Role: {role}");
        }
        catch (SecurityTokenExpiredException ex)
        {
            Console.WriteLine($"[AuthHandler] Token expired: {ex.Message}");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Token expired");
            return true;
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"[AuthHandler] Token invalid: {ex.Message}");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid or expired token");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthHandler] Unexpected error: {ex.Message}");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error in AuthHandler");
            return true;
        }

        return false;
    }
}