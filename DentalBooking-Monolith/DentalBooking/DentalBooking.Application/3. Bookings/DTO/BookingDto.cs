using DentalBooking.Domain.Enums;

namespace DentalBooking.Application.Bookings.DTO;

public class BookingDto
{
    public int Id { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;

    public int ProcedureId { get; set; }
    public string ProcedureName { get; set; } = string.Empty;

    public decimal ProcedurePrice { get; set; }
    public double ProcedureDurationMinutes { get; set; }

    public BookingStatus Status { get; set; }
}
