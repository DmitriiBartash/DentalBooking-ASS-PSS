using DentalBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DentalBooking.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedRolesAndAdminAsync(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        ILogger logger)
    {
        foreach (var role in new[] { "Admin", "Client" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(role));
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}: {Errors}",
                        role, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        var adminEmail = "admin@dental.com";
        var adminPassword = config["Seed:AdminPassword"] ?? "Admin123!";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                Phone = "+1000000000",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            else
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else
        {
            adminUser.FirstName = "System";
            adminUser.LastName = "Admin";
            adminUser.Phone = "+1000000000";
            await userManager.UpdateAsync(adminUser);
        }

        var clientPassword = config["Seed:ClientPassword"] ?? "Client123!";
        var clients = new[]
        {
            new { Email = "client1@dental.com", FirstName = "John", LastName = "Doe", Phone = "+2000000000" },
            new { Email = "client2@dental.com", FirstName = "Anna", LastName = "Smith", Phone = "+2000000001" }
        };

        foreach (var client in clients)
        {
            var existingClient = await userManager.FindByEmailAsync(client.Email);
            if (existingClient == null)
            {
                var user = new ApplicationUser
                {
                    UserName = client.Email,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Phone = client.Phone,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, clientPassword);
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create client {Email}: {Errors}",
                        client.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    continue;
                }

                await userManager.AddToRoleAsync(user, "Client");
            }
            else
            {
                existingClient.FirstName = client.FirstName;
                existingClient.LastName = client.LastName;
                existingClient.Phone = client.Phone;
                await userManager.UpdateAsync(existingClient);
            }
        }
    }
}
