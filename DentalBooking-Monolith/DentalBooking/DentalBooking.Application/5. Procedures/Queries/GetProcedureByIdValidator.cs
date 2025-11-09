using FluentValidation;

namespace DentalBooking.Application.Procedures.Queries;

public class GetProcedureByIdValidator : AbstractValidator<GetProcedureByIdQuery>
{
    public GetProcedureByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Procedure Id must be greater than 0.");
    }
}
