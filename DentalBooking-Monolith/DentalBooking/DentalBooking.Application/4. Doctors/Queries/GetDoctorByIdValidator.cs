using FluentValidation;

namespace DentalBooking.Application.Doctors.Queries;

public class GetDoctorByIdValidator : AbstractValidator<GetDoctorByIdQuery>
{
    public GetDoctorByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Doctor Id must be greater than 0.");
    }
}
