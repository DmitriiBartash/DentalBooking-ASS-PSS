using DentalBooking.Application.Bookings.DTO;
using DentalBooking.Application.Reports.DTO;
using DentalBooking.Domain.Entities;
using DentalBooking.Domain.Enums;

namespace DentalBooking.Application.Common.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Booking booking, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> HasConflictAsync(int doctorId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken);

    Task<List<BookingDto>> GetFilteredAsync(
        string? clientId,   
        int? doctorId,
        int? procedureId,
        DateTime? fromUtc,
        DateTime? toUtc,
        BookingStatus? status,
        string? sortBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<BookingStatisticsDto> GetStatisticsAsync(
        int? doctorId,
        int? procedureId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken);
}
