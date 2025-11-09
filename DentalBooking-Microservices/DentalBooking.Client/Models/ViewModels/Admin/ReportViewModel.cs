using DentalBooking.Client.Models.ViewModels.Common;

namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class ReportViewModel
    {
        public List<SimpleItem> Doctors { get; set; } = [];
        public List<SimpleItem> Procedures { get; set; } = [];
        public BookingStatisticsViewModel? Stats { get; set; }

        public int? DoctorId { get; set; }
        public int? ProcedureId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}