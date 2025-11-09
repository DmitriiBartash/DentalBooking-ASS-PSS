using MediatR;

namespace DentalBooking.Application.Procedures.Commands;

public record DeleteProcedureCommand(int Id) : IRequest<bool>;
