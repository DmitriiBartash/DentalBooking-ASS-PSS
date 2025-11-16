using DentalBooking.AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DentalBooking.AuthService.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting database seeding...");

            var roles = new[] { "Admin", "Doctor", "Client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        logger.LogError("Failed to create role {Role}: {Errors}",
                            role, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                    else
                    {
                        logger.LogInformation("Role created: {Role}", role);
                    }
                }
            }

            await EnsureUserAsync(userManager, config, logger,
                emailKey: "Seed:AdminEmail",
                passwordKey: "Seed:AdminPassword",
                defaultEmail: "admin@dental.local",
                defaultPassword: "Admin123!",
                firstName: "System",
                lastName: "Administrator",
                phone: "+37360000000",
                role: "Admin");

            var doctors = new[]
            {
                new { Email = "doctor1@dental.local", FirstName = "John", LastName = "Doe", Phone = "+37360000001" },
                new { Email = "doctor2@dental.local", FirstName = "Emily", LastName = "Carter", Phone = "+37360000002" }
            };

            foreach (var doc in doctors)
            {
                await EnsureUserAsync(userManager, config, logger,
                    emailKey: null,
                    passwordKey: "Seed:DoctorPassword",
                    defaultEmail: doc.Email,
                    defaultPassword: "Doctor123!",
                    firstName: doc.FirstName,
                    lastName: doc.LastName,
                    phone: doc.Phone,
                    role: "Doctor");
            }

            var clients = new[]
            {
                new { Email = "client1@dental.local", FirstName = "Alice", LastName = "Green", Phone = "+37360000003" },
                new { Email = "client2@dental.local", FirstName = "Bob", LastName = "Brown", Phone = "+37360000004" }
            };

            foreach (var c in clients)
            {
                await EnsureUserAsync(userManager, config, logger,
                    emailKey: null,
                    passwordKey: "Seed:ClientPassword",
                    defaultEmail: c.Email,
                    defaultPassword: "Client123!",
                    firstName: c.FirstName,
                    lastName: c.LastName,
                    phone: c.Phone,
                    role: "Client");
            }

            logger.LogInformation("Seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during seeding process");
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        ILogger logger,
        string? emailKey,
        string? passwordKey,
        string defaultEmail,
        string defaultPassword,
        string firstName,
        string lastName,
        string phone,
        string role)
    {
        var email = emailKey != null ? config[emailKey] ?? defaultEmail : defaultEmail;
        var password = passwordKey != null ? config[passwordKey] ?? defaultPassword : defaultPassword;

        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            logger.LogInformation("User already exists: {Email}", email);
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            PhoneNumberConfirmed = true,
            Role = role,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create {Role} {Email}: {Errors}",
                role, email, string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, role);
        logger.LogInformation("Created {Role}: {Email}", role, email);
    }
}
