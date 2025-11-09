using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class BookingsFilterViewModel
    {
        public int? DoctorId { get; set; }
        public int? ProcedureId { get; set; }
        public string? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public IEnumerable<SelectListItem> Doctors { get; set; } = [];
        public IEnumerable<SelectListItem> Procedures { get; set; } = [];
        public IEnumerable<BookingViewModel> Bookings { get; set; } = [];
    }
}
