using FluentValidation;

namespace DentalBooking.Application.Auth.Commands;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");
    }
}
