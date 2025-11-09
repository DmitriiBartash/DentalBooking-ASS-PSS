using FluentValidation;

namespace DentalBooking.Application.Procedures.Commands;

public class DeleteProcedureValidator : AbstractValidator<DeleteProcedureCommand>
{
    public DeleteProcedureValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Procedure Id must be greater than 0.");
    }
}
