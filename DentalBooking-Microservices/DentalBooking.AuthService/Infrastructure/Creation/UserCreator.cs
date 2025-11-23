using DentalBooking.AuthService.Domain.Entities;

namespace DentalBooking.AuthService.Infrastructure.Creation;

public abstract class UserCreator : IUserCreator
{
    public abstract ApplicationUser CreateUser(string email, string firstName, string lastName);
}
