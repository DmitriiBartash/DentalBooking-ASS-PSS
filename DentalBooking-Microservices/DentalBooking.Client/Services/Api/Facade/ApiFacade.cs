using DentalBooking.Client.Models.Api.Common;
using DentalBooking.Client.Models.Dto;
using DentalBooking.Client.Models.ViewModels.Admin;
using DentalBooking.Client.Models.ViewModels.Common;
using DentalBooking.Client.Services.Api.Endpoints;

namespace DentalBooking.Client.Services.Api.Facade;

public class ApiFacade(
    AuthApiService authService,
    BookingApiService bookingService,
    DoctorApiService doctorService,
    ProcedureApiService procedureService,
    ReportsApiService reportsService)
    : IApiFacade
{
    public AuthApiService Auth => authService;
    public DoctorApiService Doctors => doctorService;
    public ProcedureApiService Procedures => procedureService;
    public BookingApiService Bookings => bookingService;
    public ReportsApiService Reports => reportsService;

    /// <summary>
    /// Creates a booking and returns a combined summary (doctor + procedure + schedule).
    /// Implements Template + Facade pattern synergy.
    /// </summary>
    public async Task<BookingSummaryDto?> CreateBookingWithDetailsAsync(CreateBookingDto dto)
    {
        var response = await bookingService.CreateAsync(dto.ClientId, dto.DoctorId, dto.ProcedureId, dto.StartUtc);
        if (response?.Data == null)
            return null;

        var doctor = await doctorService.GetByIdAsync(dto.DoctorId);
        var procedure = await procedureService.GetByIdAsync(dto.ProcedureId);

        return new BookingSummaryDto
        {
            BookingId = response.Data,
            DoctorName = doctor != null ? $"{doctor.FirstName} {doctor.LastName}".Trim() : "Unknown",
            ProcedureName = procedure?.Name ?? "Unknown",
            Start = dto.StartUtc
        };
    }

    public async Task<List<BookingItem>> GetClientBookingsAsync(string clientId)
    {
        var bookings = await bookingService.GetByClientIdAsync(clientId);
        if (bookings == null || bookings.Count == 0)
            return [];

        var doctorIds = bookings.Select(b => b.DoctorId).Distinct().ToList();
        var procedureIds = bookings.Select(b => b.ProcedureId).Distinct().ToList();

        var doctors = await doctorService.GetAllAsync();
        var procedures = await procedureService.GetAllAsync();

        foreach (var booking in bookings)
        {
            var doctor = doctors.FirstOrDefault(d => d.Id == booking.DoctorId);
            var procedure = procedures.FirstOrDefault(p => p.Id == booking.ProcedureId);

            booking.DoctorName = doctor != null
                ? $"{doctor.FirstName} {doctor.LastName}".Trim()
                : "Unknown";

            booking.ProcedureName = procedure?.Name ?? "Unknown";
        }

        return bookings;
    }

    public async Task<List<BookingViewModel>> GetAdminBookingsAsync(
       int? doctorId = null,
       int? procedureId = null,
       string? status = null,
       DateTime? from = null,
       DateTime? to = null)
    {
        var bookings = await bookingService.GetAllAsync(doctorId, procedureId, status, from, to);
        if (bookings.Count == 0)
            return [];

        var doctors = await doctorService.GetAllAsync();
        var procedures = await procedureService.GetAllAsync();

        return [.. bookings.Select(b =>
        {
            var doctor = doctors.FirstOrDefault(d => d.Id == b.DoctorId);
            var procedure = procedures.FirstOrDefault(p => p.Id == b.ProcedureId);

            return new BookingViewModel
            {
                Id = b.Id,
                ClientEmail = b.ClientName ?? b.ClientId,
                DoctorName = doctor != null ? $"{doctor.FirstName} {doctor.LastName}".Trim() : "Unknown",
                ProcedureName = procedure?.Name ?? "Unknown",
                Date = b.StartUtc.ToLocalTime(),
                Status = b.Status
            };
        })];
    }

    public async Task<ApiMessageResponse> CancelBookingAsync(int bookingId)
    {
        var existingBookings = await bookingService.GetAllAsync();
        if (existingBookings.All(b => b.Id != bookingId))
            return new ApiMessageResponse { Message = "Booking not found." };

        var result = await bookingService.DeleteAsync(bookingId);
        return new ApiMessageResponse
        {
            Message = result?.Message ?? "Failed to cancel booking."
        };
    }

    public async Task<List<BookingItem>> GetAllBookingsAsync() => await bookingService.GetAllAsync();

    public async Task<ApiMessageResponse> ChangeBookingStatusAsync(int id, string newStatus)
    {
        var ok = await bookingService.ChangeStatusAsync(id, newStatus);
        return new ApiMessageResponse
        {
            Message = ok?.Message ?? "Failed to update booking status."
        };
    }
}
