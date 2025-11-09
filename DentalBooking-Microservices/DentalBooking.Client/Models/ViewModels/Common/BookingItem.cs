namespace DentalBooking.Client.Models.ViewModels.Common
{
    public class BookingItem
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public int ProcedureId { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string ProcedureName { get; set; } = string.Empty;
        public DateTime StartUtc { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
