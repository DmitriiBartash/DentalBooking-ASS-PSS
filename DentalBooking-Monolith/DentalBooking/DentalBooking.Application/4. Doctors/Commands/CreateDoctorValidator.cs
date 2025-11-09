using FluentValidation;

namespace DentalBooking.Application.Doctors.Commands;

public class CreateDoctorValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Doctor name is required.")
            .MaximumLength(100).WithMessage("Doctor name must be at most 100 characters.");

        RuleFor(x => x.SelectedProcedureIds)
            .NotNull().WithMessage("At least one procedure must be selected.")
            .Must(p => p.Count > 0).WithMessage("At least one procedure must be selected.")
            .Must(p => p.Count <= 3).WithMessage("A doctor can have at most 3 procedures.");
    }
}
