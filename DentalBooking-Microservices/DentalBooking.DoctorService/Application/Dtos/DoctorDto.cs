namespace DentalBooking.DoctorService.Application.Dtos;

public class DoctorDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public List<string> Procedures { get; set; } = [];

    public DoctorDto() { }

    public DoctorDto(int id, string firstName, string lastName, string specialty)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Specialty = specialty;
    }

    public DoctorDto(int id, string firstName, string lastName, string specialty, List<string> procedures)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Specialty = specialty;
        Procedures = procedures;
    }
}
