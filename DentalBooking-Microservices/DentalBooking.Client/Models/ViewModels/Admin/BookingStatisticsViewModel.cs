namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class BookingStatisticsViewModel
    {
        public int TotalBookings { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> ByProcedure { get; set; } = [];
    }
}
