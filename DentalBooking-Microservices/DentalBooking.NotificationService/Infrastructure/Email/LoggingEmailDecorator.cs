namespace DentalBooking.NotificationService.Infrastructure.Email;

public class LoggingEmailDecorator(IEmailSender inner, ILogger<LoggingEmailDecorator> logger) : EmailSenderDecorator(inner)
{
    private readonly ILogger<LoggingEmailDecorator> _logger = logger;

    public override async Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation("[LOG] Starting email send to {To} (Subject: {Subject})", to, subject);
        await base.SendAsync(to, subject, body);
        _logger.LogInformation("[LOG] Email successfully sent to {To}", to);
    }
}
