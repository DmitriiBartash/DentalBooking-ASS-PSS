namespace DentalBooking.Domain.Entities;

/// <summary>
/// Represents a medical procedure with duration and price.
/// Can be assigned to doctors and booked by clients.
/// </summary>
public class Procedure
{
    public int Id { get; set; }

    public string Code { get; set; } = default!;   // Example: A, B, C, D, E
    public string Name { get; set; } = default!;
    public TimeSpan Duration { get; set; }         // How long the procedure takes
    public decimal Price { get; set; }             // Cost of the procedure

    public ICollection<DoctorProcedure> DoctorProcedures { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}
