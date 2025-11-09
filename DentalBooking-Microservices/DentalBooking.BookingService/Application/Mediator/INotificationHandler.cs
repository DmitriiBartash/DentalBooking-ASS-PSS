namespace DentalBooking.BookingService.Application.Mediator;

public interface INotificationHandler<TNotification> where TNotification : INotification
{
    Task HandleAsync(TNotification notification);
}
