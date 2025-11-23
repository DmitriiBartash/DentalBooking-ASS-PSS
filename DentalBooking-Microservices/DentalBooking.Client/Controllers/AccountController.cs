using System.IdentityModel.Tokens.Jwt;
using DentalBooking.Client.Models.ViewModels.Account;
using DentalBooking.Client.Services.Api.Facade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalBooking.Client.Controllers;

public class AccountController(IApiFacade apiFacade) : Controller
{
    [HttpGet, AllowAnonymous]
    public IActionResult Login()
    {
        if (Request.Cookies.TryGetValue("AuthToken", out var token))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("Reports", "Admin");

                if (string.Equals(role, "Client", StringComparison.OrdinalIgnoreCase))
                    return RedirectToAction("CreateBooking", "Client");

                Response.Cookies.Delete("AuthToken");
            }
            catch
            {
                Response.Cookies.Delete("AuthToken");
            }
        }

        return View(new LoginViewModel());
    }

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var payload = new { email = model.Email, password = model.Password };
        var authResult = await apiFacade.Auth.LoginAsync(payload);

        if (authResult == null || string.IsNullOrEmpty(authResult.Token))
        {
            model.ErrorMessage = "Invalid email or password.";
            return View(model);
        }

        Response.Cookies.Append("AuthToken", authResult.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(2)
        });

        return authResult.Role switch
        {
            "Admin" => RedirectToAction("Reports", "Admin"),
            "Client" => RedirectToAction("CreateBooking", "Client"),
            _ => RedirectToAction(nameof(Login))
        };
    }

    // To do: email confirmation 
    [HttpGet, AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var payload = new
        {
            firstName = model.FirstName,
            lastName = model.LastName,
            email = model.Email,
            password = model.Password,
            role = "Client",
            phoneNumber = model.PhoneNumber
        };

        var result = await apiFacade.Auth.RegisterAsync(payload);

        if (result == null)
        {
            model.ErrorMessage = "Registration failed. Please check your data.";
            return View(model);
        }

        var loginPayload = new { email = model.Email, password = model.Password };
        var authResult = await apiFacade.Auth.LoginAsync(loginPayload);

        if (authResult != null && !string.IsNullOrEmpty(authResult.Token))
        {
            Response.Cookies.Append("AuthToken", authResult.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            return authResult.Role switch
            {
                "Admin" => RedirectToAction("Reports", "Admin"),
                "Client" => RedirectToAction("CreateBooking", "Client"),
                _ => RedirectToAction(nameof(Login))
            };
        }

        TempData["SuccessMessage"] = "Account created successfully! Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet, AllowAnonymous]
    public IActionResult EmailConfirmed() => View();

    [HttpPost, Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        TempData["SuccessMessage"] = "You have been logged out.";
        return RedirectToAction(nameof(Login));
    }
}
