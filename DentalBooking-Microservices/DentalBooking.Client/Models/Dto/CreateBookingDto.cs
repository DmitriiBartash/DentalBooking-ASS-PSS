namespace DentalBooking.Client.Models.Dto;

public class CreateBookingDto
{
    public string ClientId { get; set; } = default!;
    public int DoctorId { get; set; }
    public int ProcedureId { get; set; }
    public DateTime StartUtc { get; set; }
}
