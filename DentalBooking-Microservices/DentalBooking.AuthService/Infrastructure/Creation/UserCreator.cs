using DentalBooking.AuthService.Domain.Entities;

namespace DentalBooking.AuthService.Infrastructure.Creation;

public abstract class UserCreator
{
    public abstract ApplicationUser CreateUser(string email, string firstName, string lastName);
}

public class ClientUserCreator : UserCreator
{
    public override ApplicationUser CreateUser(string email, string firstName, string lastName) =>
        new()
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            Role = "Client"
        };
}

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

public class DoctorUserCreator : UserCreator
{
    public override ApplicationUser CreateUser(string email, string firstName, string lastName) =>
        new()
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            Role = "Doctor"
        };
}

public class UserCreatorSelector
{
    public UserCreator GetCreator(string role) =>
        role switch
        {
            "Admin" => new AdminUserCreator(),
            "Doctor" => new DoctorUserCreator(),
            _ => new ClientUserCreator()
        };
}
