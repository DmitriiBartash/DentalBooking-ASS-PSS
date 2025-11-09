namespace DentalBooking.BookingService.Application.Mediator
{
    public class Mediator(IServiceProvider serviceProvider) : IMediator
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            await handler.HandleAsync(command);
        }

        public async Task<TResult> SendAsync<TResult>(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);
            return await handler.HandleAsync((dynamic)command);
        }

        public async Task PublishAsync<TNotification>(TNotification notification)
            where TNotification : INotification
        {
            var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
            foreach (var handler in handlers)
                await handler.HandleAsync(notification);
        }
    }
}
