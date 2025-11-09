using Microsoft.AspNetCore.Identity;

namespace DentalBooking.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Phone { get; set; } = default!;
}
