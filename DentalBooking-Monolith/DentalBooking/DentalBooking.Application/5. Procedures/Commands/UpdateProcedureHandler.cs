using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Procedures.Commands;

public class UpdateProcedureHandler(IProcedureRepository repo) : IRequestHandler<UpdateProcedureCommand, bool>
{
    private readonly IProcedureRepository _repo = repo;

    public async Task<bool> Handle(UpdateProcedureCommand request, CancellationToken cancellationToken)
    {
        var proc = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (proc == null) return false;

        proc.Code = request.Code;
        proc.Name = request.Name;
        proc.Duration = request.Duration;
        proc.Price = request.Price;

        await _repo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
