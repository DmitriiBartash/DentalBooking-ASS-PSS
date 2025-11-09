using DentalBooking.Application.Bookings.DTO;
using DentalBooking.Domain.Enums;
using MediatR;

namespace DentalBooking.Application.Bookings.Queries;

public record GetBookingsQuery(
    string? ClientId = null,
    int? DoctorId = null,
    int? ProcedureId = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    BookingStatus? Status = null,
    string? SortBy = "date",
    int Page = 1,
    int PageSize = 20
) : IRequest<List<BookingDto>>;
