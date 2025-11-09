using System.ComponentModel.DataAnnotations;
using DentalBooking.Client.Models.ViewModels.Common;

namespace DentalBooking.Client.Models.ViewModels.Admin
{
    public class CreateDoctorViewModel
    {
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, Display(Name = "Doctor Type")]
        [RegularExpression("^(Therapist|Surgeon)$", ErrorMessage = "Type must be either Therapist or Surgeon.")]
        public string Type { get; set; } = "Therapist";

        [Display(Name = "Selected Procedures (max 3)")]
        public List<int> SelectedProcedureIds { get; set; } = [];

        public List<ProcedureItem> AllProcedures { get; set; } = [];

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
