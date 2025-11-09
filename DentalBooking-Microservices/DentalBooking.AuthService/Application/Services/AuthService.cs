using DentalBooking.AuthService.Application.Dtos;
using DentalBooking.AuthService.Application.Interfaces;
using DentalBooking.AuthService.Domain.Entities;
using DentalBooking.AuthService.Infrastructure.Creation;
using DentalBooking.AuthService.Infrastructure.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.AuthService.Application.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
    UserCreatorSelector creatorSelector,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly UserCreatorSelector _creatorSelector = creatorSelector;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto)
    {
        _logger.LogInformation("Registering new user: {Email} with role {Role}", dto.Email, dto.Role);

        var creator = _creatorSelector.GetCreator(dto.Role);
        var user = creator.CreateUser(dto.Email, dto.FirstName, dto.LastName);
        user.PhoneNumber = dto.PhoneNumber;

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Failed to register {Email}: {Errors}", dto.Email, errors);
            throw new InvalidOperationException($"Failed to register user: {errors}");
        }

        await _userManager.AddToRoleAsync(user, user.Role);

        var token = _jwtProvider.GenerateToken(user);
        _logger.LogInformation("User {Email} registered successfully", dto.Email);

        return new AuthResponseDto
        {
            Email = user.Email!,
            Role = user.Role,
            Token = token,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        _logger.LogInformation("Attempting login for {Email}", email);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("User not found: {Email}", email);
            return null;
        }

        var isValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isValid)
        {
            _logger.LogWarning("Invalid password for {Email}", email);
            return null;
        }

        var token = _jwtProvider.GenerateToken(user);
        _logger.LogInformation("Generated JWT for {Email}", email);

        return token;
    }

    public async Task<UserProfileDto?> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("Retrieving user by email: {Email}", email);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("User not found: {Email}", email);
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Client";

        return new UserProfileDto
        {
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = role
        };
    }

    public async Task<UserProfileDto?> GetProfileAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Profile not found for {Email}", email);
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Client";

        return new UserProfileDto
        {
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = role
        };
    }

    public async Task<bool> UpdateProfileAsync(string email, UserProfileUpdateDto dto)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Cannot update profile: user {Email} not found", email);
            return false;
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Failed to update user {Email}: {Errors}", email, errors);
            return false;
        }

        _logger.LogInformation("Profile updated successfully for {Email}", email);
        return true;
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(string id)
    {
        _logger.LogInformation("Retrieving user by ID: {Id}", id);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User not found by ID: {Id}", id);
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Client";

        return new UserProfileDto
        {
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = role
        };
    }

    public async Task<List<UserProfileDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("Retrieving all users");

        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserProfileDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Client";

            result.Add(new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = role
            });
        }

        return result;
    }


}
