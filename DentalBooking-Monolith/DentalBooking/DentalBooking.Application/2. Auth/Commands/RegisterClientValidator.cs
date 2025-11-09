using FluentValidation;

namespace DentalBooking.Application.Auth.Commands;

public class RegisterClientValidator : AbstractValidator<RegisterClientCommand>
{
    public RegisterClientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters long.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must be valid (10-15 digits, optional +).");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
