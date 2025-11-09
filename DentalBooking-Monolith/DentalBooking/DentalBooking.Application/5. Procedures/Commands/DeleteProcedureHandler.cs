using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Procedures.Commands;

public class DeleteProcedureHandler(IProcedureRepository procedureRepo) : IRequestHandler<DeleteProcedureCommand, bool>
{
    private readonly IProcedureRepository _procedureRepo = procedureRepo;

    public async Task<bool> Handle(DeleteProcedureCommand request, CancellationToken cancellationToken)
    {
        var procedure = await _procedureRepo.GetByIdAsync(request.Id, cancellationToken);
        if (procedure == null) return false;

        await _procedureRepo.DeleteAsync(procedure, cancellationToken);
        await _procedureRepo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
