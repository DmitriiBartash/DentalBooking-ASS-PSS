using MediatR;

namespace DentalBooking.Application.Procedures.Commands;

public record CreateProcedureCommand(string Code, string Name, TimeSpan Duration, decimal Price) : IRequest<int>;
