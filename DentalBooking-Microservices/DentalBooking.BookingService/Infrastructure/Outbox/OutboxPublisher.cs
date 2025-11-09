using DentalBooking.BookingService.Infrastructure.Messaging;

namespace DentalBooking.BookingService.Infrastructure.Outbox;

public class OutboxPublisher(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisher> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OutboxPublisher> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPublisher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            var rabbit = scope.ServiceProvider.GetRequiredService<IRabbitPublisher>();

            var messages = await outbox.GetUnprocessedAsync();

            foreach (var msg in messages)
            {
                try
                {
                    await rabbit.PublishAsync(msg.Type, msg.Payload);
                    await outbox.MarkAsProcessedAsync(msg.Id);
                    _logger.LogInformation("Published outbox message {MessageId}", msg.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish message {MessageId}", msg.Id);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
