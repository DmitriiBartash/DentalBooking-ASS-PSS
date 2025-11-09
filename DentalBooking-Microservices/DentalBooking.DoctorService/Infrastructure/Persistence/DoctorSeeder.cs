using DentalBooking.DoctorService.Domain.Entities;
using DentalBooking.DoctorService.Domain.Patterns.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DentalBooking.DoctorService.Infrastructure.Persistence;

public static class DoctorSeeder
{
    public static async Task SeedAsync(DoctorDbContext ctx, ILogger logger, IServiceProvider sp)
    {
        await ctx.Database.MigrateAsync();

        if (await ctx.Doctors.AnyAsync())
        {
            logger.LogInformation("Doctor database already seeded.");
            return;
        }

        logger.LogInformation("Seeding initial doctors and procedures...");

        var procedures = new[]
        {
            new Procedure { Code = "A", Name = "Cleaning", Duration = TimeSpan.FromMinutes(30), Price = 500 },
            new Procedure { Code = "B", Name = "Filling", Duration = TimeSpan.FromMinutes(45), Price = 800 },
            new Procedure { Code = "C", Name = "Extraction", Duration = TimeSpan.FromMinutes(60), Price = 1000 },
            new Procedure { Code = "D", Name = "Implant", Duration = TimeSpan.FromMinutes(90), Price = 2500 },
            new Procedure { Code = "E", Name = "Root Canal", Duration = TimeSpan.FromMinutes(75), Price = 1800 },
            new Procedure { Code = "F", Name = "Whitening", Duration = TimeSpan.FromMinutes(40), Price = 1200 }
        };

        await ctx.Procedures.AddRangeAsync(procedures);
        await ctx.SaveChangesAsync();

        var surgeonFactory = AbstractFactoryRegistry.GetFactory("Surgeon", sp);
        var therapistFactory = AbstractFactoryRegistry.GetFactory("Therapist", sp);

        var surgeons = new[]
        {
            surgeonFactory.Create("John", "Smith"),
            surgeonFactory.Create("Alice", "Johnson"),
            surgeonFactory.Create("Michael", "Brown"),
            surgeonFactory.Create("Laura", "White")
        };

        var therapists = new[]
        {
            therapistFactory.Create("Emily", "Carter"),
            therapistFactory.Create("Robert", "Green"),
            therapistFactory.Create("Olivia", "Adams"),
            therapistFactory.Create("Ethan", "Scott")
        };

        await ctx.Doctors.AddRangeAsync(surgeons);
        await ctx.Doctors.AddRangeAsync(therapists);
        await ctx.SaveChangesAsync();

        logger.LogInformation("Doctor database seeded successfully with linked default procedures.");
    }
}
