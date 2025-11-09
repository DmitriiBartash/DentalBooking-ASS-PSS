using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Bookings.Commands;

public class UpdateBookingStatusHandler(IBookingRepository repo) : IRequestHandler<UpdateBookingStatusCommand, bool>
{
    private readonly IBookingRepository _repo = repo;

    public async Task<bool> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _repo.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null) return false;

        booking.Status = request.NewStatus;
        await _repo.SaveChangesAsync(cancellationToken);

        return true;
    }
}
