using DentalBooking.Application.Procedures.DTO;
using MediatR;

namespace DentalBooking.Application.Procedures.Queries;

public record GetProcedureByIdQuery(int Id) : IRequest<ProcedureDto?>;
