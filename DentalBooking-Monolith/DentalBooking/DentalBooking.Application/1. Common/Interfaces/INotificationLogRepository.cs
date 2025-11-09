using DentalBooking.Domain.Entities;

namespace DentalBooking.Application.Common.Interfaces;

public interface INotificationLogRepository
{
    Task<bool> ExistsAsync(int bookingId, string type, CancellationToken cancellationToken);
    Task AddAsync(NotificationLog log, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
