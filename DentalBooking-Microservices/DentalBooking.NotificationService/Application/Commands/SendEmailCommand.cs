using DentalBooking.NotificationService.Infrastructure.Email;

namespace DentalBooking.NotificationService.Application.Commands;

public class SendEmailCommand(IEmailSender emailSender, string to, string subject, string body) : ICommand
{
    private readonly IEmailSender _emailSender = emailSender;
    private readonly string _to = to;
    private readonly string _subject = subject;
    private readonly string _body = body;

    public async Task ExecuteAsync()
    {
        await _emailSender.SendAsync(_to, _subject, _body);
    }
}
