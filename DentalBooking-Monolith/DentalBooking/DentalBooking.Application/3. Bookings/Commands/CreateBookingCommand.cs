using MediatR;

namespace DentalBooking.Application.Bookings.Commands;

public record CreateBookingCommand(
    string ClientId,
    int DoctorId,
    int ProcedureId,
    DateTime StartUtc
) : IRequest<int>;
