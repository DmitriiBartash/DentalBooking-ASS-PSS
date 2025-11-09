namespace DentalBooking.NotificationService.Application.Dtos;

public class BookingCreatedMessage
{
    public int BookingId { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientPhone { get; set; }
    public DateTime StartUtc { get; set; }
}
