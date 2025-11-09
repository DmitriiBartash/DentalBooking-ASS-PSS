using System.ComponentModel.DataAnnotations.Schema;

namespace DentalBooking.BookingService.Domain.Events;

[NotMapped]
public class BookingCreatedEvent(int bookingId, DateTime startUtc, string? clientEmail, string? clientPhone) : BaseDomainEvent
{
    public int BookingId { get; } = bookingId;
    public DateTime StartUtc { get; } = startUtc;
    public string? ClientEmail { get; } = clientEmail;
    public string? ClientPhone { get; } = clientPhone;
}
