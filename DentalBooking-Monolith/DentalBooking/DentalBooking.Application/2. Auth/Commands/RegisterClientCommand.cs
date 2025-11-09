using MediatR;

namespace DentalBooking.Application.Auth.Commands;

public record RegisterClientCommand(
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    string Password
) : IRequest<string>;
