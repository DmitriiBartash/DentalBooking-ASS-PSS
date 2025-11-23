namespace DentalBooking.AuthService.Infrastructure.Creation;

public class UserCreatorSelector(
    ClientUserCreator clientCreator,
    AdminUserCreator adminCreator,
    DoctorUserCreator doctorCreator)
{
    private readonly ClientUserCreator _clientCreator = clientCreator;
    private readonly AdminUserCreator _adminCreator = adminCreator;
    private readonly DoctorUserCreator _doctorCreator = doctorCreator;

    public IUserCreator GetCreator(string role) =>
        role switch
        {
            "Admin" => _adminCreator,
            "Doctor" => _doctorCreator,
            _ => _clientCreator
        };
}
