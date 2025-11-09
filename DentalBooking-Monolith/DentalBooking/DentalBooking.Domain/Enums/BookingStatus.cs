namespace DentalBooking.Domain.Enums;

/// <summary>
/// Possible states of a booking.
/// </summary>
public enum BookingStatus
{
    Pending,    // Created but not confirmed yet
    Confirmed,  // Confirmed booking (future or waiting to be done)
    Cancelled,  // Cancelled booking
    Completed   // Successfully completed procedure
}
