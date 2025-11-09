using System.ComponentModel.DataAnnotations;
using DentalBooking.Client.Models.ViewModels.Common;

namespace DentalBooking.Client.Models.ViewModels.Admin;

public class EditDoctorViewModel
{
    public int Id { get; set; }

    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, Display(Name = "Type")]
    [RegularExpression("^(Therapist|Surgeon)$", ErrorMessage = "Type must be either 'Therapist' or 'Surgeon'.")]
    public string Type { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "Select at least 1 procedure.")]
    [MaxLength(3, ErrorMessage = "You can select maximum 3 procedures.")]
    public List<int> SelectedProcedureIds { get; set; } = [];

    public List<ProcedureItem> AllProcedures { get; set; } = [];

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
