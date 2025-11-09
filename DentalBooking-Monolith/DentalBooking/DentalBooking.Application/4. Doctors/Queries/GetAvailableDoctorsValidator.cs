using FluentValidation;

namespace DentalBooking.Application.Doctors.Queries;

public class GetAvailableDoctorsValidator : AbstractValidator<GetAvailableDoctorsQuery>
{
    public GetAvailableDoctorsValidator()
    {
        RuleFor(x => x.ProcedureId)
            .GreaterThan(0).WithMessage("ProcedureId must be greater than zero.");

        RuleFor(x => x.StartUtc)
            .GreaterThan(DateTime.UtcNow).WithMessage("Start time must be in the future.");
    }
}
