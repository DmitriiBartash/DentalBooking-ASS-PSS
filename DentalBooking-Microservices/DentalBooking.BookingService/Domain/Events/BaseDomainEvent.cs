using DentalBooking.BookingService.Application.Mediator;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalBooking.BookingService.Domain.Events;

[NotMapped]
public abstract class BaseDomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
