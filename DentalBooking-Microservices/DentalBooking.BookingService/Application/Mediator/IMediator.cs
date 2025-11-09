namespace DentalBooking.BookingService.Application.Mediator
{
    public interface IMediator
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> SendAsync<TResult>(ICommand command);
        Task PublishAsync<TNotification>(TNotification notification) where TNotification : INotification;
    }
}
