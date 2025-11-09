using System.Drawing;
using System.Net.Mail;

namespace DentalBooking.NotificationService.Infrastructure.Email;

public class EmailSender(ILogger<EmailSender> logger, IConfiguration config)
{
    private readonly ILogger<EmailSender> _logger = logger;
    private readonly IConfiguration _config = config;

    public async Task SendAsync(string to, string subject, string body)
    {
        var smtpServer = _config["EmailSettings:SmtpServer"] ?? "localhost";
        var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? "25");
        var senderAddress = _config["EmailSettings:SenderAddress"] ?? "noreply@dentalease.com";
        var senderName = _config["EmailSettings:SenderName"] ?? "DentalEase";

        try
        {
            using var smtp = new SmtpClient(smtpServer, smtpPort)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var htmlBody = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: 'Segoe UI', Arial, sans-serif;
                        background-color: #f9fafb;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        background: white;
                        max-width: 600px;
                        margin: 40px auto;
                        padding: 25px;
                        border-radius: 12px;
                        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                    }}
                    h2 {{
                        color: #2b5b84;
                    }}
                    p {{
                        font-size: 15px;
                        color: #333;
                        line-height: 1.6;
                    }}
                    .footer {{
                        margin-top: 30px;
                        font-size: 13px;
                        color: #888;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <h2>DentalEase Appointment</h2>
                    <p>{body}</p>
                    <div class=""footer"">
                        © 2025 DentalEase Notification Service
                    </div>
                </div>
            </body>
            </html>
            ";

            using var message = new MailMessage
            {
                From = new MailAddress(senderAddress, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await smtp.SendMailAsync(message);
            _logger.LogInformation("HTML Email sent successfully to {To} via Papercut SMTP", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}
