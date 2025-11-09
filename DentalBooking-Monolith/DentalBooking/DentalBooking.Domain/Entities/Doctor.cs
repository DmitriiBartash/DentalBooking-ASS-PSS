namespace DentalBooking.Domain.Entities;

/// <summary>
/// Represents a doctor who can perform medical procedures.
/// </summary>
public class Doctor
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;

    public ICollection<DoctorProcedure> DoctorProcedures { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}
