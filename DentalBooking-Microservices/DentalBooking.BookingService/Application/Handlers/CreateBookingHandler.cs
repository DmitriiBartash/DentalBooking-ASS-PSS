using DentalBooking.BookingService.Application.Commands;
using DentalBooking.BookingService.Application.Mediator;
using DentalBooking.BookingService.Domain.Entities;
using DentalBooking.BookingService.Domain.Events;
using DentalBooking.BookingService.Infrastructure.External;
using DentalBooking.BookingService.Infrastructure.Persistence;

namespace DentalBooking.BookingService.Application.Handlers;

public class CreateBookingHandler(
    IBookingRepository repo,
    IMediator mediator,
    IAuthApiClient authClient) : ICommandHandler<CreateBookingCommand, int>
{
    public async Task<int> HandleAsync(CreateBookingCommand command)
    {
        var booking = Booking.Create(
            command.DoctorId,
            command.ClientId,
            command.ProcedureId,
            command.StartUtc);

        await repo.AddAsync(booking);
        await repo.SaveChangesAsync();

        var contact = await authClient.GetClientContactAsync(command.ClientId);

        await mediator.PublishAsync(new BookingCreatedEvent(
            booking.Id,
            booking.StartUtc,
            contact?.Email,
            contact?.Phone)
        );

        return booking.Id; 
    }
}
