using DentalBooking.Application.Bookings.DTO;
using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Application.Reports.DTO;
using DentalBooking.Domain.Entities;
using DentalBooking.Domain.Enums;
using DentalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.Infrastructure.Repositories;

public class BookingRepository(AppDbContext context) : IBookingRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => await _context.Bookings.FindAsync([id], cancellationToken);

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
        => await _context.Bookings.AddAsync(booking, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<bool> HasConflictAsync(int doctorId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken)
        => await _context.Bookings
            .AnyAsync(b =>
                b.DoctorId == doctorId &&
                b.Status != BookingStatus.Cancelled &&
                b.StartUtc < endUtc &&
                b.EndUtc > startUtc, cancellationToken);

    public async Task<List<BookingDto>> GetByClientAsync(
        string clientId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await _context.Bookings
            .Include(b => b.Procedure)
            .Include(b => b.Doctor)
            .Where(b => b.ClientId == clientId)
            .OrderByDescending(b => b.StartUtc)
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                ClientId = b.ClientId,
                ClientEmail = _context.Users
                    .Where(u => u.Id == b.ClientId)
                    .Select(u => u.Email!)
                    .FirstOrDefault() ?? string.Empty,
                DoctorId = b.DoctorId,
                DoctorName = b.Doctor.FullName,
                ProcedureId = b.ProcedureId,
                ProcedureName = b.Procedure.Name,
                ProcedurePrice = b.Procedure.Price,
                ProcedureDurationMinutes = b.Procedure.Duration.TotalMinutes,
                Status = b.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BookingDto>> GetFilteredAsync(
        string? clientId,
        int? doctorId,
        int? procedureId,
        DateTime? fromUtc,
        DateTime? toUtc,
        BookingStatus? status,
        string? sortBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Bookings
            .Include(b => b.Procedure)
            .Include(b => b.Doctor)
            .AsQueryable();

        if (!string.IsNullOrEmpty(clientId))
            query = query.Where(b => b.ClientId == clientId);

        if (doctorId.HasValue)
            query = query.Where(b => b.DoctorId == doctorId.Value);

        if (procedureId.HasValue)
            query = query.Where(b => b.ProcedureId == procedureId.Value);

        if (fromUtc.HasValue)
            query = query.Where(b => b.StartUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(b => b.EndUtc <= toUtc.Value);

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        query = sortBy switch
        {
            "date" => query.OrderBy(b => b.StartUtc),
            "doctor" => query.OrderBy(b => b.Doctor.FullName),
            _ => query.OrderByDescending(b => b.StartUtc)
        };

        return await query
            .Skip(Math.Max(page - 1, 0) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                ClientId = b.ClientId,
                ClientEmail = _context.Users
                    .Where(u => u.Id == b.ClientId)
                    .Select(u => u.Email!)
                    .FirstOrDefault() ?? string.Empty,
                DoctorId = b.DoctorId,
                DoctorName = b.Doctor.FullName,
                ProcedureId = b.ProcedureId,
                ProcedureName = b.Procedure.Name,
                ProcedurePrice = b.Procedure.Price,
                ProcedureDurationMinutes = b.Procedure.Duration.TotalMinutes,
                Status = b.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<BookingStatisticsDto> GetStatisticsAsync(
        int? doctorId,
        int? procedureId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var query = _context.Bookings
            .Include(b => b.Procedure)
            .AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(b => b.DoctorId == doctorId.Value);

        if (procedureId.HasValue)
            query = query.Where(b => b.ProcedureId == procedureId.Value);

        if (fromUtc.HasValue)
            query = query.Where(b => b.StartUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(b => b.EndUtc <= toUtc.Value);

        var bookings = await query.ToListAsync(cancellationToken);

        var total = bookings.Count;
        var cancelled = bookings.Count(b => b.Status == BookingStatus.Cancelled);
        var completed = bookings.Count(b => b.Status == BookingStatus.Completed);

        var totalMinutes = bookings
            .Where(b => b.Status == BookingStatus.Completed)
            .Sum(b => b.Procedure.Duration.TotalMinutes);

        var revenue = bookings
            .Where(b => b.Status == BookingStatus.Completed)
            .Sum(b => b.Procedure.Price);

        var byProcedure = bookings
            .GroupBy(b => b.Procedure.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        return new BookingStatisticsDto
        {
            TotalBookings = total,
            CancelledBookings = cancelled,
            CompletedBookings = completed,
            TotalDuration = TimeSpan.FromMinutes(totalMinutes),
            TotalRevenue = revenue,
            ByProcedure = byProcedure
        };
    }

}
