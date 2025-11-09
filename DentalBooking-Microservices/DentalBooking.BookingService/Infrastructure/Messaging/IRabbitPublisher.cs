namespace DentalBooking.BookingService.Infrastructure.Messaging;

public interface IRabbitPublisher
{
    Task PublishAsync(string type, string payload);
}
