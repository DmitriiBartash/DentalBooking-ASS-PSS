using DentalBooking.BookingService.Application.Dtos;

namespace DentalBooking.BookingService.Infrastructure.External;

public interface IAuthApiClient
{
    Task<ClientContactDto?> GetClientContactAsync(string clientId);
}
