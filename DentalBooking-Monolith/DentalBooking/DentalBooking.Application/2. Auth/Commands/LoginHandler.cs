using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Auth.Commands;

public class LoginHandler(IAuthService authService) : IRequestHandler<LoginCommand, string>
{
    private readonly IAuthService _authService = authService;

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return token;
    }
}
