using DentalBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages
{
    public class LogoutModel(SignInManager<ApplicationUser> signInManager) : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Login");
        }
    }
}
