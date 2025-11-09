using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Application.Procedures.DTO;
using MediatR;

namespace DentalBooking.Application.Procedures.Queries;

public class GetProceduresHandler(IProcedureRepository repo) : IRequestHandler<GetProceduresQuery, List<ProcedureDto>>
{
    private readonly IProcedureRepository _repo = repo;

    public async Task<List<ProcedureDto>> Handle(GetProceduresQuery request, CancellationToken cancellationToken)
    {
        var procedures = await _repo.GetAllAsync(cancellationToken);

        return [.. procedures.Select(p => new ProcedureDto(
            p.Id,
            p.Code,
            p.Name,
            p.Price,
            p.Duration.TotalMinutes
        ))];
    }
}
