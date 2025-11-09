using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DentalBooking.Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<(bool Succeeded, string UserId, string ErrorMessage, string Token)> CreateClientAsync(
        string firstName,
        string lastName,
        string phone,
        string email,
        string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, string.Empty, string.Join("; ", result.Errors.Select(e => e.Description)), string.Empty);

        await _userManager.AddToRoleAsync(user, "Client");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return (true, user.Id, string.Empty, token);
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<string> GetEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Email ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        return await _userManager.IsInRoleAsync(user, role);
    }
}
