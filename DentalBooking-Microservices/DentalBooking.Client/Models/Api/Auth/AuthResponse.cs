namespace DentalBooking.Client.Models.Api.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
}
