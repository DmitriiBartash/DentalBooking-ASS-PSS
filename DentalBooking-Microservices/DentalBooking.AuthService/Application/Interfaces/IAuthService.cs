using DentalBooking.AuthService.Application.Dtos;

namespace DentalBooking.AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto);
    Task<string?> LoginAsync(string email, string password);
    Task<UserProfileDto?> GetUserByEmailAsync(string email);
    Task<UserProfileDto?> GetProfileAsync(string email);
    Task<bool> UpdateProfileAsync(string email, UserProfileUpdateDto dto);
    Task<UserProfileDto?> GetUserByIdAsync(string id);
    Task<List<UserProfileDto>> GetAllUsersAsync();

}
