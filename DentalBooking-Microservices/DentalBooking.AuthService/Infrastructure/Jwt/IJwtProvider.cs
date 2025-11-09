using DentalBooking.AuthService.Domain.Entities;

namespace DentalBooking.AuthService.Infrastructure.Jwt
{
    public interface IJwtProvider
    {
        string GenerateToken(ApplicationUser user);
    }
}
