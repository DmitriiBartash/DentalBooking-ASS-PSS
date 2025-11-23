using DentalBooking.AuthService.Domain.Entities;

namespace DentalBooking.AuthService.Infrastructure.Creation;

public class AdminUserCreator : UserCreator
{
    public override ApplicationUser CreateUser(string email, string firstName, string lastName) =>
        new()
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            Role = "Admin"
        };
}
