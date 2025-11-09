using DentalBooking.BookingService.Application.Dtos;

namespace DentalBooking.BookingService.Infrastructure.External;

public class AuthApiClient(HttpClient http, ILogger<AuthApiClient> logger) : IAuthApiClient
{
    public async Task<ClientContactDto?> GetClientContactAsync(string clientId)
    {
        try
        {
            var url = $"http://dental_authservice:8080/api/users/{Uri.EscapeDataString(clientId)}/contact";
            return await http.GetFromJsonAsync<ClientContactDto>(url);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch client contact for {ClientId}", clientId);
            return null;
        }
    }
}
