using DentalBooking.BookingService.Application.Mediator;
using DentalBooking.BookingService.Domain.Events;
using DentalBooking.BookingService.Infrastructure.Outbox;

namespace DentalBooking.BookingService.Application.Notifications;

public class OutboxEventHandler(IOutboxRepository outbox) :
    INotificationHandler<BookingCreatedEvent>,
    INotificationHandler<BookingCancelledEvent>
{
    private readonly IOutboxRepository _outbox = outbox;

    public async Task HandleAsync(BookingCreatedEvent notification)
    {
        var payload = new
        {
            notification.BookingId,
            notification.StartUtc,
            notification.ClientEmail,
            notification.ClientPhone
        };

        await _outbox.AddAsync("BookingCreatedEvent", payload);
    }

    public async Task HandleAsync(BookingCancelledEvent notification)
    {
        var payload = new
        {
            notification.Booking.Id,
            notification.Booking.ClientId,
            notification.Booking.StartUtc
        };

        await _outbox.AddAsync("BookingCancelledEvent", payload);
    }
}
