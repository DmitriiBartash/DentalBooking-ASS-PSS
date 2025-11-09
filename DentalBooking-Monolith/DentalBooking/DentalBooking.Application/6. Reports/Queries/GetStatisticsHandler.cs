using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Application.Reports.DTO;
using MediatR;

namespace DentalBooking.Application.Reports.Queries;

public class GetStatisticsHandler(IBookingRepository bookingRepo) : IRequestHandler<GetStatisticsQuery, BookingStatisticsDto>
{
    private readonly IBookingRepository _bookingRepo = bookingRepo;

    public async Task<BookingStatisticsDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _bookingRepo.GetStatisticsAsync(
            request.DoctorId,
            request.ProcedureId,
            request.FromUtc,
            request.ToUtc,
            cancellationToken);
    }
}
