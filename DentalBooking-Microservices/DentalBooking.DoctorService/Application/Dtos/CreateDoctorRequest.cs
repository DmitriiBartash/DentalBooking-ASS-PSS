using System.ComponentModel.DataAnnotations;

namespace DentalBooking.DoctorService.Application.Dtos;

public class CreateDoctorRequest
{
    [Required(ErrorMessage = "First name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Doctor type is required.")]
    [RegularExpression("^(Therapist|Surgeon)$", ErrorMessage = "Type must be either 'Therapist' or 'Surgeon'.")]
    public string Type { get; set; } = "Therapist";

    [Display(Name = "Procedure IDs")]
    public List<int>? ProcedureIds { get; set; }
}
