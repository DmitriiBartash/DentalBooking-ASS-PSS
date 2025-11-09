using FluentValidation;

namespace DentalBooking.Application.Bookings.Commands;

public class UpdateBookingStatusValidator : AbstractValidator<UpdateBookingStatusCommand>
{
    public UpdateBookingStatusValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0).WithMessage("BookingId must be greater than 0.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid booking status.");
    }
}
