using DentalBooking.Client.Models.Api;
using DentalBooking.Client.Models.Api.Common;
using DentalBooking.Client.Models.Dto;
using DentalBooking.Client.Models.ViewModels.Common;
using DentalBooking.Client.Services.Api.Endpoints;

namespace DentalBooking.Client.Services.Api.Facade;

public interface IApiFacade
{
    AuthApiService Auth { get; }
    DoctorApiService Doctors { get; }
    ProcedureApiService Procedures { get; }
    BookingApiService Bookings { get; }
    ReportsApiService Reports { get; }

    Task<BookingSummaryDto?> CreateBookingWithDetailsAsync(CreateBookingDto dto);
    Task<List<BookingItem>> GetClientBookingsAsync(string clientId);
    Task<List<BookingItem>> GetAllBookingsAsync();
    Task<ApiMessageResponse> CancelBookingAsync(int bookingId);
    Task<ApiMessageResponse> ChangeBookingStatusAsync(int id, string newStatus);
}
