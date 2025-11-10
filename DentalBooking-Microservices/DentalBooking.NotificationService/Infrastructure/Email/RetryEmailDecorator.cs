namespace DentalBooking.NotificationService.Infrastructure.Email;

public class RetryEmailDecorator(IEmailSender inner, ILogger<RetryEmailDecorator> logger) : EmailSenderDecorator(inner)
{
    private readonly ILogger<RetryEmailDecorator> _logger = logger;
    private const int MaxRetries = 3;

    public override async Task SendAsync(string to, string subject, string body)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await base.SendAsync(to, subject, body);
                _logger.LogInformation("[RETRY] Email sent successfully to {To} on attempt {Attempt}", to, attempt);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[RETRY] Attempt {Attempt} failed to send email to {To}", attempt, to);
                if (attempt == MaxRetries)
                {
                    _logger.LogError("[RETRY] All {MaxRetries} attempts failed for {To}", MaxRetries, to);
                    throw;
                }
                await Task.Delay(TimeSpan.FromSeconds(2 * attempt)); 
            }
        }
    }
}
