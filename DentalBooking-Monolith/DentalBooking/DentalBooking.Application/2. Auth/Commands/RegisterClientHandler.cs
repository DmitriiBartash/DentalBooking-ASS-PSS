using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Auth.Commands;

public class RegisterClientHandler(
    IUserService userService,
    IEmailSender emailSender,
    IAppSettingsProvider appSettingsProvider)
    : IRequestHandler<RegisterClientCommand, string>
{
    private readonly IUserService _userService = userService;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IAppSettingsProvider _appSettingsProvider = appSettingsProvider;

    public async Task<string> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, errorMessage, token) = await _userService.CreateClientAsync(
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Email,
            request.Password
        );

        if (!succeeded)
            throw new InvalidOperationException(errorMessage);

        var confirmationLink =
            $"{_appSettingsProvider.FrontendBaseUrl}/api/auth/confirm?userId={userId}&token={Uri.EscapeDataString(token)}";

        var body = $@"
            <h2>Welcome to DentalBooking!</h2>
            <p>Please confirm your registration by clicking the link below:</p>
            <p><a href='{confirmationLink}'>Confirm Email</a></p>";

        await _emailSender.SendEmailAsync(request.Email, "Confirm your email", body);

        return "Registration successful. Please confirm your email.";
    }
}
