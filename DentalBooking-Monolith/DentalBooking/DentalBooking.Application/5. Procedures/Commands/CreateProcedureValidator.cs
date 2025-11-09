using FluentValidation;

namespace DentalBooking.Application.Procedures.Commands;

public class CreateProcedureValidator : AbstractValidator<CreateProcedureCommand>
{
    public CreateProcedureValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(10).WithMessage("Code must not exceed 10 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Duration)
            .Must(d => d.TotalMinutes > 0 && d.TotalMinutes <= 300)
            .WithMessage("Duration must be between 1 and 300 minutes.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Price must not exceed 10,000.");
    }
}
