using DentalBooking.BookingService.Domain.Entities;
using DentalBooking.BookingService.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.BookingService.Infrastructure.Persistence;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.DoctorId).IsRequired();
            entity.Property(b => b.ClientId).IsRequired();
            entity.Property(b => b.ProcedureId).IsRequired();
            entity.Property(b => b.StartUtc).IsRequired();
            entity.Property(b => b.Status).IsRequired();
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Type).IsRequired();
            entity.Property(o => o.Payload).IsRequired();
            entity.Property(o => o.CreatedUtc).IsRequired();
        });
    }
}
