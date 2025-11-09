using Microsoft.AspNetCore.Identity;

namespace DentalBooking.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole() : base() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
