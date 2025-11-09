using FluentValidation;

namespace DentalBooking.Application.Doctors.Commands;

public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Doctor ID must be greater than zero.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.SelectedProcedureIds)
            .NotNull().WithMessage("At least one procedure must be selected.")
            .Must(list => list.Count <= 3)
            .WithMessage("A doctor can perform maximum 3 procedures.");
    }
}
