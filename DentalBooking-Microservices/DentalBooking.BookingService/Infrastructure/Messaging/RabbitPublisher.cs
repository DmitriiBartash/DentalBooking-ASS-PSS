using System.Text;
using RabbitMQ.Client;

namespace DentalBooking.BookingService.Infrastructure.Messaging;

public class RabbitPublisher : IRabbitPublisher
{
    private readonly ILogger<RabbitPublisher> _logger;

    public RabbitPublisher(ILogger<RabbitPublisher> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync(string type, string payload)
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: type,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var body = Encoding.UTF8.GetBytes(payload);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: type,
            body: body
        );

        _logger.LogInformation("📨 [RabbitMQ] Published event '{Type}'", type);
    }
}
