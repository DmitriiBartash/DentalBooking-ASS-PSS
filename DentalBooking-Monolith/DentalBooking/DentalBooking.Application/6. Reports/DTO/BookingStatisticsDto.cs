namespace DentalBooking.Application.Reports.DTO;

public class BookingStatisticsDto
{
    public int TotalBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int CompletedBookings { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public decimal TotalRevenue { get; set; }   
    public Dictionary<string, int> ByProcedure { get; set; } = [];
}
