using DentalBooking.BookingService.Application.Mediator;

namespace DentalBooking.BookingService.Application.Commands;

public class CancelBookingCommand(int bookingId) : ICommand
{
    public int BookingId { get; } = bookingId;
}
