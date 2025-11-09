using DentalBooking.Client.Models.ViewModels.Common;

namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class DoctorsViewModel
    {
        public List<DoctorItem> Doctors { get; set; } = [];
        public List<ProcedureItem> Procedures { get; set; } = []; 

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
