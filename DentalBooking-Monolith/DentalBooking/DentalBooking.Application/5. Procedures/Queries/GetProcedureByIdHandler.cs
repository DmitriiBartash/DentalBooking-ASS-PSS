using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Application.Procedures.DTO;
using MediatR;

namespace DentalBooking.Application.Procedures.Queries;

public class GetProcedureByIdHandler(IProcedureRepository repo) : IRequestHandler<GetProcedureByIdQuery, ProcedureDto?>
{
    private readonly IProcedureRepository _repo = repo;

    public async Task<ProcedureDto?> Handle(GetProcedureByIdQuery request, CancellationToken cancellationToken)
    {
        var proc = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (proc == null) return null;

        return new ProcedureDto(
            proc.Id,
            proc.Code,
            proc.Name,
            proc.Price,
            proc.Duration.TotalMinutes
        );
    }
}
