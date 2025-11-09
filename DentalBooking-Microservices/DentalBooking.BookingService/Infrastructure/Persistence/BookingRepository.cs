using DentalBooking.BookingService.Domain.Entities;
using DentalBooking.BookingService.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.BookingService.Infrastructure.Persistence;

public class BookingRepository(DataContext context) : IBookingRepository
{
    private readonly DataContext _context = context;

    public async Task AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id) => await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task<IEnumerable<int>> GetBusyDoctorsAsync(IEnumerable<int> doctorIds, DateTime startUtc)
    {
        return await _context.Bookings
            .Where(b => doctorIds.Contains(b.DoctorId)
                        && b.StartUtc == startUtc
                        && b.Status == BookingStatus.Confirmed)
            .Select(b => b.DoctorId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<Booking>> GetAllAsync(
        int? doctorId = null,
        string? clientId = null,
        int? procedureId = null,
        string? status = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        var query = _context.Bookings.AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(b => b.DoctorId == doctorId.Value);

        if (!string.IsNullOrEmpty(clientId))
            query = query.Where(b => b.ClientId == clientId);

        if (procedureId.HasValue)
            query = query.Where(b => b.ProcedureId == procedureId.Value);

        if (!string.IsNullOrEmpty(status)
            && Enum.TryParse<BookingStatus>(status, true, out var parsedStatus))
            query = query.Where(b => b.Status == parsedStatus);

        if (from.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
            query = query.Where(b => b.StartUtc >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
            query = query.Where(b => b.StartUtc <= toUtc);
        }

        return await query.ToListAsync();
    }
}
