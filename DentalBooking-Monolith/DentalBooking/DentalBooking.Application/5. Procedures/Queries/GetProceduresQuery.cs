using DentalBooking.Application.Procedures.DTO;
using MediatR;

namespace DentalBooking.Application.Procedures.Queries;

public record GetProceduresQuery() : IRequest<List<ProcedureDto>>;
