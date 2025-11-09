namespace DentalBooking.BookingService.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; }
}
