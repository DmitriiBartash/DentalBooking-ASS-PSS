using System.Text;
using DentalBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace DentalBooking.Web.Pages;

public class ConfirmEmailModel(UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [TempData] public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            StatusMessage = "Invalid confirmation link.";
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            StatusMessage = "User not found.";
            return Page();
        }

        var decodedBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        StatusMessage = result.Succeeded
            ? "Your email has been confirmed! You can now log in."
            : string.Join("; ", result.Errors.Select(e => e.Description));

        return Page();
    }
}
