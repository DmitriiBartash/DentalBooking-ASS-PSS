using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using MediatR;

namespace DentalBooking.Application.Procedures.Commands;

public class CreateProcedureHandler(IProcedureRepository procedureRepo) : IRequestHandler<CreateProcedureCommand, int>
{
    private readonly IProcedureRepository _procedureRepo = procedureRepo;

    public async Task<int> Handle(CreateProcedureCommand request, CancellationToken cancellationToken)
    {
        var procedure = new Procedure
        {
            Code = request.Code,
            Name = request.Name,
            Duration = request.Duration,
            Price = request.Price
        };

        await _procedureRepo.AddAsync(procedure, cancellationToken);
        await _procedureRepo.SaveChangesAsync(cancellationToken);

        return procedure.Id;
    }
}
