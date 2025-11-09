namespace DentalBooking.Application.Procedures.DTO;

public record ProcedureDto(
    int Id,
    string Code,
    string Name,
    decimal Price,
    double DurationMinutes
);
