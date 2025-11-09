using FluentValidation;

namespace DentalBooking.Application.Doctors.Commands;

public class AssignDoctorProceduresValidator : AbstractValidator<AssignDoctorProceduresCommand>
{
    public AssignDoctorProceduresValidator()
    {
        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("DoctorId must be greater than zero.");

        RuleFor(x => x.ProcedureIds)
            .NotEmpty().WithMessage("At least one procedure must be assigned.")
            .Must(ids => ids.Count <= 3)
            .WithMessage("A doctor cannot have more than 3 procedures.");

        RuleForEach(x => x.ProcedureIds)
            .GreaterThan(0).WithMessage("ProcedureId must be greater than zero.");
    }
}
