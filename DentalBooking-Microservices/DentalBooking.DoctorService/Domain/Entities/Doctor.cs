namespace DentalBooking.DoctorService.Domain.Entities;

public class Doctor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public ICollection<DoctorProcedure> DoctorProcedures { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}".Trim();
}

