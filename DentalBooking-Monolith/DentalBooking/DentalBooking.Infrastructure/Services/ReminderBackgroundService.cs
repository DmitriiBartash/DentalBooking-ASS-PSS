using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using DentalBooking.Domain.Enums;
using DentalBooking.Infrastructure.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DentalBooking.Infrastructure.Services;

public class ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<ReminderBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationLogRepository>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var targetDate = DateTime.UtcNow.AddDays(1).Date;

            var bookings = await bookingRepo.GetFilteredAsync(
                clientId: null,
                doctorId: null,
                procedureId: null,
                fromUtc: targetDate,
                toUtc: targetDate.AddDays(1),
                status: BookingStatus.Confirmed,
                sortBy: "date",
                page: 1,
                pageSize: 1000,
                cancellationToken: stoppingToken);

            int sent = 0, skipped = 0, failed = 0;

            foreach (var booking in bookings)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(booking.ClientEmail) || booking.StartUtc <= DateTime.UtcNow ||
                        await notificationRepo.ExistsAsync(booking.Id, "Reminder", stoppingToken))
                    {
                        skipped++;
                        continue;
                    }

                    var body = EmailTemplates.Reminder(
                        booking.DoctorName,
                        booking.ProcedureName,
                        booking.StartUtc);

                    await emailSender.SendEmailAsync(
                        booking.ClientEmail,
                        "Appointment Reminder",
                        body);

                    await notificationRepo.AddAsync(new NotificationLog
                    {
                        BookingId = booking.Id,
                        Email = booking.ClientEmail,
                        Type = "Reminder",
                        SentAtUtc = DateTime.UtcNow
                    }, stoppingToken);

                    await notificationRepo.SaveChangesAsync(stoppingToken);
                    sent++;
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger.LogError(ex, "Failed to send reminder for booking {BookingId}", booking.Id);
                }
            }

            _logger.LogInformation("Reminder cycle completed: {Sent} sent, {Skipped} skipped, {Failed} failed",
                sent, skipped, failed);

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
