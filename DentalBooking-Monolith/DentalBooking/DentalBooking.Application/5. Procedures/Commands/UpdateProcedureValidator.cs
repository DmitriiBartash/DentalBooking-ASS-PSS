using FluentValidation;

namespace DentalBooking.Application.Procedures.Commands;

public class UpdateProcedureValidator : AbstractValidator<UpdateProcedureCommand>
{
    public UpdateProcedureValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(10);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than 0.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");
    }
}
