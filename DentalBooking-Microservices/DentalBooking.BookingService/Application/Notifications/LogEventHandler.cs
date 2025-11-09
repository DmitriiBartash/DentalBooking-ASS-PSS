using DentalBooking.BookingService.Application.Mediator;
using DentalBooking.BookingService.Domain.Events;

namespace DentalBooking.BookingService.Application.Notifications;

public class LogEventHandler(ILogger<LogEventHandler> logger) :
    INotificationHandler<BookingCreatedEvent>,
    INotificationHandler<BookingCancelledEvent>
{
    private readonly ILogger<LogEventHandler> _logger = logger;

    public Task HandleAsync(BookingCreatedEvent notification)
    {
        _logger.LogInformation(
            "[BookingCreated] BookingId={BookingId}, StartUtc={StartUtc}, Email={Email}, Phone={Phone}",
            notification.BookingId,
            notification.StartUtc,
            notification.ClientEmail ?? "none",
            notification.ClientPhone ?? "none");

        return Task.CompletedTask;
    }

    public Task HandleAsync(BookingCancelledEvent notification)
    {
        _logger.LogInformation(
            "[BookingCancelled] BookingId={BookingId}, ClientId={ClientId}, StartUtc={StartUtc}",
            notification.Booking.Id,
            notification.Booking.ClientId,
            notification.Booking.StartUtc);

        return Task.CompletedTask;
    }
}
