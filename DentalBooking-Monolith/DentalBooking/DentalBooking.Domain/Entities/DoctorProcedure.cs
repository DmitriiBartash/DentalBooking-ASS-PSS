namespace DentalBooking.Domain.Entities;

/// <summary>
/// Join entity for many-to-many relationship between Doctor and Procedure.
/// </summary>
public class DoctorProcedure
{
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;

    public int ProcedureId { get; set; }
    public Procedure Procedure { get; set; } = default!;
}
