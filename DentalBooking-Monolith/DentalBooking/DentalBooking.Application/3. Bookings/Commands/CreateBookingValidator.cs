using FluentValidation;

namespace DentalBooking.Application.Bookings.Commands;

public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId is required.");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("DoctorId must be greater than 0.");

        RuleFor(x => x.ProcedureId)
            .GreaterThan(0).WithMessage("ProcedureId must be greater than 0.");

        RuleFor(x => x.StartUtc)
            .NotEmpty().WithMessage("StartUtc is required.")
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Booking time must be in the future.");
    }
}
