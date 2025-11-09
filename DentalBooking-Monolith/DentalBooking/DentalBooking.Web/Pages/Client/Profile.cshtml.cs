using DentalBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace DentalBooking.Web.Pages.Client;

public class ProfileModel(UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData] public string? SuccessMessage { get; set; }
    [TempData] public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Login");
        }

        Input = new InputModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            Email = user.Email!
        };

        ViewData["ActivePage"] = "Profile";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["ActivePage"] = "Profile";
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            ErrorMessage = "User not found.";
            return RedirectToPage("/Login");
        }

        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.Phone = Input.Phone;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            ViewData["ActivePage"] = "Profile";
            return Page();
        }

        SuccessMessage = "Profile updated successfully!";
        return RedirectToPage();
    }
}
