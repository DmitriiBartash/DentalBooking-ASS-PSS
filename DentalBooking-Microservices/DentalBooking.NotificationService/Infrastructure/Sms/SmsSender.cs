using Microsoft.Extensions.Logging;

namespace DentalBooking.NotificationService.Infrastructure.Sms;

public class SmsSender(ILogger<SmsSender> logger)
{
    private readonly ILogger<SmsSender> _logger = logger;

    private const string Yellow = "\u001b[33m";
    private const string Green = "\u001b[32m";
    private const string Red = "\u001b[31m";
    private const string Reset = "\u001b[0m";

    public Task SendAsync(string phone, string message)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            _logger.LogWarning("{Color}SMS not sent: phone number is missing{Reset}", Yellow, Reset);
            return Task.CompletedTask;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            _logger.LogWarning("{Color}SMS not sent: message is empty{Reset}", Yellow, Reset);
            return Task.CompletedTask;
        }

        _logger.LogInformation("{Color}Simulating SMS send...{Reset}", Green, Reset);
        _logger.LogInformation("{Color}[SMS SENT]{Reset} To: {Phone} | Message: {Message}", Green, Reset, phone, message);

        return Task.CompletedTask;
    }
}
