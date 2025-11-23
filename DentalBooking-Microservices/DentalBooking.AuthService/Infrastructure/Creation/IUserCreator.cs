using DentalBooking.AuthService.Domain.Entities;

namespace DentalBooking.AuthService.Infrastructure.Creation;

public interface IUserCreator
{
    ApplicationUser CreateUser(string email, string firstName, string lastName);
}
