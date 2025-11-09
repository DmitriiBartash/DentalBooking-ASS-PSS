using DentalBooking.BookingService.Domain.Enums;
using DentalBooking.BookingService.Domain.Events;

namespace DentalBooking.BookingService.Domain.Entities;

public class Booking
{
    public int Id { get; private set; }
    public int DoctorId { get; private set; }
    public int ProcedureId { get; private set; }
    public string ClientId { get; private set; } = default!;
    public DateTime StartUtc { get; private set; }
    public BookingStatus Status { get; private set; }

    private readonly List<BaseDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Booking() { }

    private Booking(int doctorId, string clientId, int procedureId, DateTime startUtc)
    {
        DoctorId = doctorId;
        ClientId = clientId;
        ProcedureId = procedureId;
        StartUtc = startUtc;
        Status = BookingStatus.Created;
    }

    public static Booking Create(int doctorId, string clientId, int procedureId, DateTime startUtc)
    {
        if (startUtc < DateTime.UtcNow)
            throw new InvalidOperationException("Cannot create booking in the past.");

        return new Booking(doctorId, clientId, procedureId, startUtc);
    }

    public void Confirm()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Cannot confirm a cancelled booking.");
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status is BookingStatus.Cancelled or BookingStatus.Completed)
            throw new InvalidOperationException("Booking cannot be cancelled.");
        Status = BookingStatus.Cancelled;
        AddEvent(new BookingCancelledEvent(this));
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be completed.");
        Status = BookingStatus.Completed;
    }

    private void AddEvent(BaseDomainEvent e) => _domainEvents.Add(e);
}
