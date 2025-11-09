using System.ComponentModel.DataAnnotations;

namespace DentalBooking.Client.Models.ViewModels.Account;

public class RegisterViewModel
{
    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
