using System.ComponentModel.DataAnnotations;
using DentalBooking.AuthService.Application.Dtos;
using DentalBooking.AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DentalBooking.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        _logger.LogInformation("Received register request for {Email}", dto.Email);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("Invalid registration data for {Email}: {Errors}",
                dto.Email, string.Join("; ", errors));

            return BadRequest(new
            {
                message = "Validation failed",
                errors
            });
        }

        try
        {
            var result = await _authService.RegisterAsync(dto);

            if (result == null)
            {
                _logger.LogWarning("Registration failed for {Email}: AuthService returned null", dto.Email);
                return BadRequest(new { message = "Registration failed" });
            }

            _logger.LogInformation("User {Email} registered successfully with role {Role}",
                dto.Email, result.Role);

            return Ok(new
            {
                message = "User registered successfully",
                result.Email,
                result.FirstName,
                result.LastName,
                result.Role,
                result.Token
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration error for {Email}: {Message}", dto.Email, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for {Email}", dto.Email);
            return StatusCode(500, new { message = "Unexpected server error" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token == null)
        {
            _logger.LogWarning("Invalid login attempt for {Email}", dto.Email);
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var user = await _authService.GetUserByEmailAsync(dto.Email);
        if (user == null)
        {
            _logger.LogWarning("User not found after successful login for {Email}", dto.Email);
            return Unauthorized(new { message = "User not found." });
        }

        var response = new AuthResponseDto
        {
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = token
        };

        _logger.LogInformation("User {Email} logged in successfully", dto.Email);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Profile access attempt without valid token");
            return Unauthorized(new { message = "Invalid or missing token." });
        }

        var profile = await _authService.GetProfileAsync(email);
        if (profile == null)
        {
            _logger.LogWarning("Profile not found for {Email}", email);
            return NotFound(new { message = "User not found." });
        }

        _logger.LogInformation("Profile retrieved successfully for {Email}", email);
        return Ok(profile);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto dto)
    {
        _logger.LogInformation("Received profile update request from {User}", User.Identity?.Name);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("Invalid profile update data: {Errors}", string.Join("; ", errors));
            return BadRequest(new
            {
                message = "Validation failed",
                errors
            });
        }

        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Unauthorized(new { message = "Invalid or missing token." });

        try
        {
            var success = await _authService.UpdateProfileAsync(email, dto);
            if (!success)
            {
                _logger.LogWarning("Profile update failed for {Email}", email);
                return BadRequest(new { message = "Profile update failed." });
            }

            var updated = await _authService.GetProfileAsync(email);
            _logger.LogInformation("Profile updated successfully for {Email}", email);

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during profile update for {Email}", email);
            return StatusCode(500, new { message = "Unexpected server error" });
        }
    }
}
