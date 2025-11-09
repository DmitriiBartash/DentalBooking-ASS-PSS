using DentalBooking.Domain.Enums;

namespace DentalBooking.Domain.Entities;

/// <summary>
/// Represents a scheduled medical procedure for a patient.
/// </summary>
public class Booking
{
    public int Id { get; set; }

    public string ClientId { get; set; } = default!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public int ProcedureId { get; set; }
    public Procedure Procedure { get; set; } = default!;

    public DateTime StartUtc { get; set; }  
    public DateTime EndUtc { get; set; }    

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
}
