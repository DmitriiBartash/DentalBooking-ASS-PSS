using DentalBooking.BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DentalBooking.BookingService.Infrastructure.Persistence;

public static class BookingSeeder
{
    public static async Task SeedAsync(DataContext db, ILogger logger)
    {
        if (await db.Bookings.AnyAsync())
        {
            logger.LogInformation("BookingSeeder: bookings already exist, skipping seeding.");
            return;
        }

        logger.LogInformation("BookingSeeder: seeding test bookings...");

        var now = DateTime.UtcNow;

        var booking1 = Booking.Create(
            doctorId: 1,
            clientId: "c1-test-client",
            procedureId: 1,
            startUtc: now.AddDays(1).Date.AddHours(9));

        var booking2 = Booking.Create(
            doctorId: 1,
            clientId: "c2-test-client",
            procedureId: 2,
            startUtc: now.AddDays(2).Date.AddHours(11));
        booking2.Confirm();

        var booking3 = Booking.Create(
            doctorId: 2,
            clientId: "c1-test-client",
            procedureId: 4,
            startUtc: now.AddDays(3).Date.AddHours(14));
        booking3.Confirm();
        booking3.Complete();

        var booking4 = Booking.Create(
            doctorId: 2,
            clientId: "c3-test-client",
            procedureId: 6,
            startUtc: now.AddDays(4).Date.AddHours(10));
        booking4.Cancel();

        db.Bookings.AddRange(booking1, booking2, booking3, booking4);
        await db.SaveChangesAsync();

        logger.LogInformation("BookingSeeder: seeding completed.");
    }
}
