namespace DentalBooking.BookingService.Application.Mediator
{
    public interface ICommand { }

    public interface ICommand<TResult> : ICommand { }
}
