namespace DentalBooking.Application.Common.Interfaces;

public interface IUserService
{
    Task<(bool Succeeded, string UserId, string ErrorMessage, string Token)> CreateClientAsync(
        string firstName,
        string lastName,
        string phone,
        string email,
        string password);

    Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken);

    Task<bool> IsEmailConfirmedAsync(string userId);

    Task<string> GetEmailAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);
}
