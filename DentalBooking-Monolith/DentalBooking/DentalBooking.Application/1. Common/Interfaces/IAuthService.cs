namespace DentalBooking.Application.Common.Interfaces;

public interface IAuthService
{
    Task<string?> LoginAsync(string email, string password);
}
