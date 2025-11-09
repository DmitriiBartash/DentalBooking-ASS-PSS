using DentalBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace DentalBooking.Web.Pages;

public class LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; } = false;

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null || !user.EmailConfirmed)
        {
            ErrorMessage = "Invalid login attempt or email not confirmed.";
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            Password,
            RememberMe,
            lockoutOnFailure: true
        );

        if (result.Succeeded)
        {
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToPage("/Admin/Reports");

            if (await _userManager.IsInRoleAsync(user, "Client"))
                return RedirectToPage("/Client/CreateBooking");

            return RedirectToPage("/Login");
        }

        if (result.IsLockedOut)
        {
            ErrorMessage = "Your account is locked due to multiple failed login attempts. Please try again later.";
            return Page();
        }

        ErrorMessage = "Invalid login attempt.";
        return Page();
    }
}
