using DentalBooking.BookingService.Application.Commands;
using DentalBooking.BookingService.Application.Mediator;
using DentalBooking.BookingService.Infrastructure.Persistence;

namespace DentalBooking.BookingService.Application.Handlers;

public class CancelBookingHandler(IBookingRepository repo) : ICommandHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _repo = repo;

    public async Task HandleAsync(CancelBookingCommand command)
    {
        var booking = await _repo.GetByIdAsync(command.BookingId) ?? throw new Exception("Booking not found.");

        booking.Cancel();
        await _repo.SaveChangesAsync();
    }
}
