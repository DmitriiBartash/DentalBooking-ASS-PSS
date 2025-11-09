namespace DentalBooking.DoctorService.Domain.Entities;

public class DoctorProcedure
{
    public int DoctorId { get; set; }
    public int ProcedureId { get; set; }

    public Doctor? Doctor { get; set; }
    public Procedure? Procedure { get; set; }
}
