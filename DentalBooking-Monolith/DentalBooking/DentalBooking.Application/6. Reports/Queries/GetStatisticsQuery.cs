using DentalBooking.Application.Reports.DTO;
using MediatR;

namespace DentalBooking.Application.Reports.Queries;

public record GetStatisticsQuery(
    int? DoctorId,
    int? ProcedureId,
    DateTime? FromUtc,
    DateTime? ToUtc
) : IRequest<BookingStatisticsDto>;
