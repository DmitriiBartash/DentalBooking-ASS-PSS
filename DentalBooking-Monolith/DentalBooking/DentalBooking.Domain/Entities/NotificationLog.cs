namespace DentalBooking.Domain.Entities;

/// <summary>
/// Stores information about sent notifications (e.g., reminders, confirmations).
/// Used to prevent duplicate emails.
/// </summary>
public class NotificationLog
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = default!;

    public string Email { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Reminder", "Confirmation"
    public DateTime SentAtUtc { get; set; }
}
