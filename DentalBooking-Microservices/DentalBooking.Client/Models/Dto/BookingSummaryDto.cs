namespace DentalBooking.Client.Models.Dto;

public class BookingSummaryDto
{
    public int BookingId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string ProcedureName { get; set; } = string.Empty;
    public DateTime Start { get; set; }
}
