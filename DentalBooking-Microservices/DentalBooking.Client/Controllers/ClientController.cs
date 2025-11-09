using DentalBooking.Client.Models.Dto;
using DentalBooking.Client.Models.ViewModels.Client;
using DentalBooking.Client.Services.Api.Facade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalBooking.Client.Controllers;

[Authorize(Roles = "Client")]
public class ClientController(IApiFacade apiFacade) : Controller
{
    #region Booking

    [HttpGet]
    public async Task<IActionResult> CreateBooking()
    {
        ViewData["ActivePage"] = "CreateBooking";

        var procedures = await apiFacade.Procedures.GetAllAsync();
        var model = new CreateBookingViewModel
        {
            Procedures = [.. procedures.Select(p => new SelectListItem(p.Name, p.Id.ToString()))]
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingViewModel model)
    {
        ViewData["ActivePage"] = "CreateBooking";

        if (!ModelState.IsValid)
        {
            var procedures = await apiFacade.Procedures.GetAllAsync();
            model.Procedures = [.. procedures.Select(p => new SelectListItem(p.Name, p.Id.ToString()))];
            return View(model);
        }

        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            TempData["ErrorMessage"] = "User not authenticated.";
            return RedirectToAction("Login", "Account");
        }

        var startLocal = model.Date.Date + model.Time;
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal);

        if (startUtc <= DateTime.UtcNow)
        {
            TempData["ErrorMessage"] = "Booking must be in the future.";
            var procedures = await apiFacade.Procedures.GetAllAsync();
            model.Procedures = [.. procedures.Select(p => new SelectListItem(p.Name, p.Id.ToString()))];
            return View(model);
        }

        var dto = new CreateBookingDto
        {
            ClientId = userId,
            DoctorId = model.DoctorId,
            ProcedureId = model.ProcedureId,
            StartUtc = startUtc
        };

        var bookingSummary = await apiFacade.CreateBookingWithDetailsAsync(dto);
        if (bookingSummary == null)
        {
            TempData["ErrorMessage"] = "Failed to create booking.";
            return View(model);
        }

        TempData["SuccessMessage"] = $"Booking #{bookingSummary.BookingId} successfully created!";
        return RedirectToAction("MyBookings");
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        ViewData["ActivePage"] = "MyBookings";

        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var bookings = await apiFacade.GetClientBookingsAsync(userId);
        return View(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CancelBooking(int id)
    {
        ViewData["ActivePage"] = "MyBookings";

        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Invalid booking ID.";
            return RedirectToAction(nameof(MyBookings));
        }

        var success = await apiFacade.Bookings.CancelAsync(id);

        if (success)
            TempData["SuccessMessage"] = $"Booking #{id} was successfully cancelled.";
        else
            TempData["ErrorMessage"] = "Failed to cancel booking. Please try again.";

        return RedirectToAction(nameof(MyBookings));
    }

    [HttpGet]
    public async Task<IActionResult> AvailableDoctors(int procedureId, DateTime start)
    {
        var doctors = await apiFacade.Doctors.GetByProcedureAsync(procedureId);
        if (doctors == null || doctors.Count == 0)
            return Json(Array.Empty<object>());

        var doctorIds = doctors.Select(d => (int)d.Id).ToList();
        var busyIds = await apiFacade.Bookings.GetBusyDoctorsAsync(doctorIds, start);

        var available = doctors
            .Where(d => !busyIds.Contains(d.Id))
            .Select(d => new { d.Id, FullName = $"{d.FirstName} {d.LastName}" })
            .ToList();

        return Json(available);
    }

    #endregion

    #region Profile Management

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        ViewData["ActivePage"] = "Profile";

        var profile = await apiFacade.Auth.GetProfileAsync();
        if (profile == null)
        {
            TempData["ErrorMessage"] = "Failed to load profile.";
            return View(new ProfileViewModel());
        }

        return View(new ProfileViewModel
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber
        });
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        ViewData["ActivePage"] = "Profile";

        if (!ModelState.IsValid)
            return View(model);

        var result = await apiFacade.Auth.UpdateProfileAsync(model);

        if (result?.Message != null && result.Message.Contains("failed", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = result.Message ?? "Failed to update profile.";
            return View(model);
        }

        TempData["SuccessMessage"] = result?.Message ?? "Profile updated successfully!";
        return RedirectToAction(nameof(Profile));
    }

    #endregion
}
