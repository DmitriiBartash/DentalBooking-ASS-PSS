using FluentValidation;

namespace DentalBooking.Application.Doctors.Commands;

public class DeleteDoctorValidator : AbstractValidator<DeleteDoctorCommand>
{
    public DeleteDoctorValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Doctor Id must be greater than 0.");
    }
}
