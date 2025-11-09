namespace DentalBooking.Client.Models.ViewModels.Client
{
    public class ClientBookingViewModel
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string ProcedureName { get; set; } = string.Empty;
        public DateTime StartUtc { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
