using DentalBooking.BookingService.Domain.Entities;

namespace DentalBooking.BookingService.Infrastructure.Persistence;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<Booking?> GetByIdAsync(int id);
    Task SaveChangesAsync();
    Task<IEnumerable<int>> GetBusyDoctorsAsync(IEnumerable<int> doctorIds, DateTime startUtc);
    Task<List<Booking>> GetAllAsync(
        int? doctorId = null,
        string? clientId = null,
        int? procedureId = null,
        string? status = null,
        DateTime? from = null,
        DateTime? to = null);
}
