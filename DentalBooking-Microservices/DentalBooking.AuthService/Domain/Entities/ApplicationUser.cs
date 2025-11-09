using Microsoft.AspNetCore.Identity;

namespace DentalBooking.AuthService.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";
    }
}
