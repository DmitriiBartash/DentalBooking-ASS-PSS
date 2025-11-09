namespace DentalBooking.Application.Doctors.DTO;

public record DoctorDto(
    int Id,
    string FullName,
    List<int> ProcedureIds
);
