using DentalBooking.AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.AuthService.Infrastructure.Persistence;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.Role)
                .HasMaxLength(50)
                .IsRequired();
        });
    }

    // public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
