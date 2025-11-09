using DentalBooking.Application.Common.Interfaces;

namespace DentalBooking.Infrastructure.Services;

public class ConsoleEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("====================================================");
        Console.WriteLine($"FAKE EMAIL to: {email}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine("Message:");
        Console.WriteLine(htmlMessage);
        Console.WriteLine("====================================================");
        Console.ResetColor();

        return Task.CompletedTask;
    }
}
