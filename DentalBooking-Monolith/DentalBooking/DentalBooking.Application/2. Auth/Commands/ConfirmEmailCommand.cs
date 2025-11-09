using MediatR;

namespace DentalBooking.Application.Auth.Commands;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<string>;
