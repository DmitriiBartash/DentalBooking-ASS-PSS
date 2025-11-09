namespace DentalBooking.BookingService.Application.Mediator
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }
}
