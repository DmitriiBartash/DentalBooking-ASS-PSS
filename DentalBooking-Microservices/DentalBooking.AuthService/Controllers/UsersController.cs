using DentalBooking.AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DentalBooking.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IAuthService authService, ILogger<UsersController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<UsersController> _logger = logger;

    [HttpGet("{id}/contact")]
    public async Task<IActionResult> GetContact(string id)
    {
        _logger.LogInformation("Fetching contact info for user {UserId}", id);

        var user = await _authService.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", id);
            return NotFound(new { message = "User not found." });
        }

        return Ok(new
        {
            user.Email,
            Phone = user.PhoneNumber
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all users");

        var users = await _authService.GetAllUsersAsync(); 

        if (users == null || users.Count == 0)
        {
            _logger.LogWarning("No users found");
            return NotFound(new { message = "No users found." });
        }

        var result = users.Select(u => new
        {
            u.Id,
            u.Email,
            FullName = $"{u.FirstName} {u.LastName}".Trim()
        });

        return Ok(result);
    }
}
