using DentalBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DentalBooking.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<DoctorProcedure> DoctorProcedures => Set<DoctorProcedure>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Many-to-many: Doctor to Procedure
        modelBuilder.Entity<DoctorProcedure>()
            .HasKey(dp => new { dp.DoctorId, dp.ProcedureId });

        // Indexes for performance
        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.StartUtc);
        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.DoctorId);
        modelBuilder.Entity<NotificationLog>()
            .HasIndex(n => new { n.BookingId, n.Type })
            .IsUnique();

        // Seed Doctors
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { Id = 1, FullName = "Dr. John Smith" },
            new Doctor { Id = 2, FullName = "Dr. Anna Brown" },
            new Doctor { Id = 3, FullName = "Dr. Alex Johnson" },
            new Doctor { Id = 4, FullName = "Dr. Maria Davis" },
            new Doctor { Id = 5, FullName = "Dr. Peter Wilson" }
        );

        // Seed Procedures
        modelBuilder.Entity<Procedure>().HasData(
            new Procedure { Id = 1, Code = "A", Name = "Procedure A", Duration = TimeSpan.FromMinutes(30), Price = 50m },
            new Procedure { Id = 2, Code = "B", Name = "Procedure B", Duration = TimeSpan.FromMinutes(45), Price = 70m },
            new Procedure { Id = 3, Code = "C", Name = "Procedure C", Duration = TimeSpan.FromMinutes(60), Price = 100m },
            new Procedure { Id = 4, Code = "D", Name = "Procedure D", Duration = TimeSpan.FromMinutes(20), Price = 40m },
            new Procedure { Id = 5, Code = "E", Name = "Procedure E", Duration = TimeSpan.FromMinutes(90), Price = 150m }
        );


        // Seed Doctor - Procedure relationships 
        modelBuilder.Entity<DoctorProcedure>().HasData(
            new DoctorProcedure { DoctorId = 1, ProcedureId = 1 },
            new DoctorProcedure { DoctorId = 1, ProcedureId = 2 },
            new DoctorProcedure { DoctorId = 1, ProcedureId = 3 },

            new DoctorProcedure { DoctorId = 2, ProcedureId = 2 },
            new DoctorProcedure { DoctorId = 2, ProcedureId = 4 },
            new DoctorProcedure { DoctorId = 2, ProcedureId = 5 },

            new DoctorProcedure { DoctorId = 3, ProcedureId = 1 },
            new DoctorProcedure { DoctorId = 3, ProcedureId = 3 },

            new DoctorProcedure { DoctorId = 4, ProcedureId = 4 },
            new DoctorProcedure { DoctorId = 4, ProcedureId = 5 },

            new DoctorProcedure { DoctorId = 5, ProcedureId = 1 },
            new DoctorProcedure { DoctorId = 5, ProcedureId = 2 }
        );
    }
}
