using System.ComponentModel.DataAnnotations;
using System.Text;
using DentalBooking.Infrastructure.Identity;
using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Infrastructure.Templates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace DentalBooking.Web.Pages;

public class RegisterModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender) : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _email = emailSender;

    [BindProperty, Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty, Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [BindProperty, Required, Phone]
    public string Phone { get; set; } = string.Empty;

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty, Required, DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [TempData] public string? ErrorMessage { get; set; }
    [TempData] public string? SuccessMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var existingUser = await _userManager.FindByEmailAsync(Email);
        if (existingUser != null)
        {
            ErrorMessage = "User with this email already exists.";
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Email,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            Phone = Phone
        };

        var create = await _userManager.CreateAsync(user, Password);
        if (!create.Succeeded)
        {
            ErrorMessage = string.Join("; ", create.Errors.Select(e => e.Description));
            return Page();
        }

        await _userManager.AddToRoleAsync(user, "Client");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var confirmUrl = Url.Page(
            "/ConfirmEmail",
            pageHandler: null,
            values: new { userId = user.Id, token = encodedToken },
            protocol: Request.Scheme
        )!;

        await _email.SendEmailAsync(
            Email,
            "Confirm your DentalBooking account",
            EmailTemplates.RegistrationConfirmation(confirmUrl)
        );

        SuccessMessage = "Registration successful! Please check your email to confirm your account.";
        return RedirectToPage("/Login");
    }
}
