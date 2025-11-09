using System.Text;
using System.Text.Json;
using DentalBooking.NotificationService.Application.Commands;
using DentalBooking.NotificationService.Application.Dtos;
using DentalBooking.NotificationService.Application.Invokers;
using DentalBooking.NotificationService.Infrastructure.Email;
using DentalBooking.NotificationService.Infrastructure.Sms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DentalBooking.NotificationService.Infrastructure.Messaging;

public class RabbitConsumer(ILogger<RabbitConsumer> logger, IServiceProvider services) : BackgroundService
{
    private readonly ILogger<RabbitConsumer> _logger = logger;
    private readonly IServiceProvider _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "BookingCreatedEvent",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<BookingCreatedMessage>(messageBody);
                if (message is null)
                {
                    _logger.LogWarning("Received null or invalid message from RabbitMQ");
                    return;
                }

                _logger.LogInformation("Received BookingCreatedEvent for BookingId={BookingId}", message.BookingId);

                using var scope = _services.CreateScope();
                var emailSender = scope.ServiceProvider.GetRequiredService<EmailSender>();
                var smsSender = scope.ServiceProvider.GetRequiredService<SmsSender>();

                var invoker = new NotificationInvoker();

                if (!string.IsNullOrWhiteSpace(message.ClientEmail))
                {
                    invoker.SetCommand(new SendEmailCommand(emailSender, message.ClientEmail,
                        "Booking Created", $"Your booking #{message.BookingId} is scheduled for {message.StartUtc:g}."));
                    await invoker.ExecuteCommandAsync();
                }

                if (!string.IsNullOrWhiteSpace(message.ClientPhone))
                {
                    invoker.SetCommand(new SendSmsCommand(smsSender, message.ClientPhone,
                        $"Booking #{message.BookingId} confirmed for {message.StartUtc:g}."));
                    await invoker.ExecuteCommandAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing BookingCreatedEvent message");
            }
        };

        await channel.BasicConsumeAsync(
            queue: "BookingCreatedEvent",
            autoAck: true,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
