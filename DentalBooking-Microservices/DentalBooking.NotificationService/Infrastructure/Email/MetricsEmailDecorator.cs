using System.Diagnostics;

namespace DentalBooking.NotificationService.Infrastructure.Email;

public class MetricsEmailDecorator(IEmailSender inner, ILogger<MetricsEmailDecorator> logger) : EmailSenderDecorator(inner)
{
    private readonly ILogger<MetricsEmailDecorator> _logger = logger;

    public override async Task SendAsync(string to, string subject, string body)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[METRICS] Sending email to {To}", to);

        await base.SendAsync(to, subject, body);

        stopwatch.Stop();
        _logger.LogInformation("[METRICS] Email to {To} took {Elapsed} ms", to, stopwatch.ElapsedMilliseconds);
    }
}
