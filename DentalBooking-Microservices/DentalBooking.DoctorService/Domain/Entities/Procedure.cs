namespace DentalBooking.DoctorService.Domain.Entities;

public class Procedure
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public TimeSpan Duration { get; set; }
    public decimal Price { get; set; }

    public ICollection<DoctorProcedure> DoctorProcedures { get; set; } = [];
}
