using DentalBooking.NotificationService.Application.Commands;

namespace DentalBooking.NotificationService.Application.Invokers;

public class NotificationInvoker
{
    private ICommand? _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    public async Task ExecuteCommandAsync()
    {
        if (_command is not null)
            await _command.ExecuteAsync();
    }
}
