using DentalBooking.NotificationService.Application.Invokers;
using DentalBooking.NotificationService.Infrastructure.Email;
using DentalBooking.NotificationService.Infrastructure.Messaging;
using DentalBooking.NotificationService.Infrastructure.Sms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EmailSender>();
builder.Services.AddSingleton<SmsSender>();
builder.Services.AddSingleton<NotificationInvoker>();
builder.Services.AddHostedService<RabbitConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

using (var scope = app.Services.CreateScope())
{
    var emailSender = scope.ServiceProvider.GetRequiredService<EmailSender>();
    var smsSender = scope.ServiceProvider.GetRequiredService<SmsSender>();

    await emailSender.SendAsync(
        "test@localhost",
        "DentalEase SMTP Test",
        "This is a test email sent via smtp4dev to verify the Notification Service is working correctly."
    );

    await smsSender.SendAsync(
        "+37360000000",
        "This is a test SMS from DentalEase Notification Service"
    );
}

app.Run();
