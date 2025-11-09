using DentalBooking.Application.Bookings.DTO;
using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Bookings.Queries;

public class GetBookingsHandler(IBookingRepository bookingRepo)
    : IRequestHandler<GetBookingsQuery, List<BookingDto>>
{
    private readonly IBookingRepository _bookingRepo = bookingRepo;

    public async Task<List<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        return await _bookingRepo.GetFilteredAsync(
            request.ClientId,
            request.DoctorId,
            request.ProcedureId,
            request.FromUtc,
            request.ToUtc,
            request.Status,
            request.SortBy,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
