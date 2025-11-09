using MediatR;

namespace DentalBooking.Application.Procedures.Commands;

public record UpdateProcedureCommand(
    int Id,
    string Code,
    string Name,
    TimeSpan Duration,
    decimal Price
) : IRequest<bool>;
