namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string ClientEmail { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string ProcedureName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Pending";

        public string StatusClass =>
            Status switch
            {
                "Pending" => "secondary",
                "Created" => "success",      
                "Confirmed" => "info",
                "Completed" => "primary",
                "Cancelled" => "danger",
                _ => "light"
            };
    }
}
