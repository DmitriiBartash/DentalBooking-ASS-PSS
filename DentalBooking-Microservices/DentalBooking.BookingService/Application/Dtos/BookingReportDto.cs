namespace DentalBooking.BookingService.Application.Dtos;

public class BookingReportDto
{
    public int TotalBookings { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public Dictionary<string, int> ByProcedure { get; set; } = [];
}
