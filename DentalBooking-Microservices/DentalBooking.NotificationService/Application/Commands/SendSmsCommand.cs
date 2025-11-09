using DentalBooking.NotificationService.Infrastructure.Sms;

namespace DentalBooking.NotificationService.Application.Commands;

public class SendSmsCommand(SmsSender smsSender, string phone, string message) : ICommand
{
    private readonly SmsSender _smsSender = smsSender;
    private readonly string _phone = phone;
    private readonly string _message = message;

    public async Task ExecuteAsync()
    {
        await _smsSender.SendAsync(_phone, _message);
    }
}
