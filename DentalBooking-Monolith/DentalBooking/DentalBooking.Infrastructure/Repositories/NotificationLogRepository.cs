using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using DentalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.Infrastructure.Repositories;

public class NotificationLogRepository(AppDbContext context) : INotificationLogRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> ExistsAsync(int bookingId, string type, CancellationToken cancellationToken)
        => await _context.NotificationLogs.AnyAsync(
            n => n.BookingId == bookingId && n.Type == type,
            cancellationToken);

    public async Task AddAsync(NotificationLog log, CancellationToken cancellationToken)
        => await _context.NotificationLogs.AddAsync(log, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}
