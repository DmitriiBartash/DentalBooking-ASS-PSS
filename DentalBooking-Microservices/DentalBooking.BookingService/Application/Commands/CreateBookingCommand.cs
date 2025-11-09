using DentalBooking.BookingService.Application.Mediator;

namespace DentalBooking.BookingService.Application.Commands
{
    public class CreateBookingCommand : ICommand<int>
    {
        public int DoctorId { get; init; }
        public string ClientId { get; init; } = default!;
        public int ProcedureId { get; init; }
        public DateTime StartUtc { get; init; }
    }
}
