namespace DentalBooking.DoctorService.Application.Dtos;

public class CreateProcedureRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
}
