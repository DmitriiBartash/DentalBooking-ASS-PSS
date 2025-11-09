using System.ComponentModel.DataAnnotations;

namespace DentalBooking.Client.Models.ViewModels.Client;

public class ProfileViewModel
{
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;
}
