using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Auth.Commands;

public class ConfirmEmailHandler(IUserService userService) : IRequestHandler<ConfirmEmailCommand, string>
{
    private readonly IUserService _userService = userService;

    public async Task<string> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.ConfirmEmailAsync(request.UserId, request.Token, cancellationToken);
        if (!result)
            throw new InvalidOperationException("Email confirmation failed");

        return "Email confirmed successfully.";
    }
}
