namespace DentalBooking.NotificationService.Infrastructure.Email;

public abstract class EmailSenderDecorator(IEmailSender inner) : IEmailSender
{
    protected readonly IEmailSender Inner = inner;

    public virtual Task SendAsync(string to, string subject, string body) => Inner.SendAsync(to, subject, body);
}
