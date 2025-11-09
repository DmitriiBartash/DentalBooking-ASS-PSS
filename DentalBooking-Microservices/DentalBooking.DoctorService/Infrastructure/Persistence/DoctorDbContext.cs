using DentalBooking.DoctorService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.DoctorService.Infrastructure.Persistence;

public class DoctorDbContext(DbContextOptions<DoctorDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<DoctorProcedure> DoctorProcedures => Set<DoctorProcedure>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.Specialty)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasMany(d => d.DoctorProcedures)
                .WithOne(dp => dp.Doctor!)
                .HasForeignKey(dp => dp.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Procedure>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Price)
                .HasPrecision(10, 2);
        });

        modelBuilder.Entity<DoctorProcedure>(entity =>
        {
            entity.HasKey(dp => new { dp.DoctorId, dp.ProcedureId });

            entity.HasOne(dp => dp.Doctor)
                .WithMany(d => d.DoctorProcedures)
                .HasForeignKey(dp => dp.DoctorId);

            entity.HasOne(dp => dp.Procedure)
                .WithMany(p => p.DoctorProcedures)
                .HasForeignKey(dp => dp.ProcedureId);
        });
    }
}
