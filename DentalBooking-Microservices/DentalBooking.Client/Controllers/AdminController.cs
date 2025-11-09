using System.Text.RegularExpressions;
using DentalBooking.Client.Models.ViewModels.Admin;
using DentalBooking.Client.Models.ViewModels.Common;
using DentalBooking.Client.Services.Api.Facade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalBooking.Client.Controllers;


[Authorize(Roles = "Admin")]
public partial class AdminController(IApiFacade apiFacade) : Controller
{
    #region Bookings Management

    [HttpGet]
    public async Task<IActionResult> Bookings([FromQuery] BookingsFilterViewModel model)
    {
        ViewData["ActivePage"] = "Bookings";

        var doctors = await apiFacade.Doctors.GetAllAsync();
        var procedures = await apiFacade.Procedures.GetAllAsync();
        var clients = await apiFacade.Auth.GetAllUsersAsync();

        var bookings = await apiFacade.Bookings.GetAllAsync(
            model.DoctorId,
            model.ProcedureId,
            model.Status,
            model.From,
            model.To
        );

        model.Doctors = [.. doctors.Select(d => new SelectListItem($"{d.FirstName} {d.LastName}", d.Id.ToString()))];

        model.Procedures = [.. procedures.Select(p => new SelectListItem(p.Name, p.Id.ToString()))];

        model.Bookings = [.. bookings.Select(b =>
        {
            var client = clients.FirstOrDefault(c => c.Id == b.ClientId);
            var doctor = doctors.FirstOrDefault(d => d.Id == b.DoctorId);
            var procedure = procedures.FirstOrDefault(p => p.Id == b.ProcedureId);

            return new BookingViewModel
            {
                Id = b.Id,

                ClientEmail = client != null
                    ? client.FullName
                    : (!string.IsNullOrEmpty(b.ClientName)
                        ? b.ClientName
                        : $"{b.ClientId[..8]}..."),

                DoctorName = doctor != null
                    ? $"{doctor.FirstName} {doctor.LastName}"
                    : $"#{b.DoctorId}",

                ProcedureName = procedure?.Name ?? $"#{b.ProcedureId}",

                Date = b.StartUtc.ToLocalTime(),
                Status = b.Status
            };
        })];


        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int id, string newStatus)
    {
        var result = await apiFacade.ChangeBookingStatusAsync(id, newStatus);
        TempData[result.Message?.Contains("success", StringComparison.OrdinalIgnoreCase) == true ? "SuccessMessage" : "ErrorMessage"] =
            result.Message ?? "Unknown response from server.";
        return RedirectToAction(nameof(Bookings));
    }

    #endregion

    #region Reports

    [HttpGet]
    public async Task<IActionResult> Reports(int? doctorId, int? procedureId, DateTime? from, DateTime? to)
    {
        ViewData["ActivePage"] = "Reports";

        var doctors = await apiFacade.Doctors.GetAllAsync();
        var procedures = await apiFacade.Procedures.GetAllAsync();
        var stats = await apiFacade.Reports.GetStatisticsAsync(doctorId, procedureId, from, to);

        if (stats?.ByProcedure != null && stats.ByProcedure.Count != 0)
        {
            var newDict = new Dictionary<string, int>();

            foreach (var kvp in stats.ByProcedure)
            {
                var match = ProcedureIdRegex().Match(kvp.Key);
                if (match.Success && int.TryParse(match.Groups[1].Value, out var id))
                {
                    var name = procedures.FirstOrDefault(p => p.Id == id)?.Name ?? $"Procedure #{id}";
                    newDict[name] = kvp.Value;
                }
                else
                {
                    newDict[kvp.Key] = kvp.Value;
                }
            }

            stats.ByProcedure = newDict;
        }

        var model = new ReportViewModel
        {
            Doctors = [.. doctors.Select(d => new SimpleItem
        {
            Id = d.Id,
            Name = $"{d.FirstName} {d.LastName}"
        })],
            Procedures = [.. procedures.Select(p => new SimpleItem
        {
            Id = p.Id,
            Name = p.Name
        })],
            Stats = stats,
            DoctorId = doctorId,
            ProcedureId = procedureId,
            FromDate = from,
            ToDate = to
        };

        return View(model);
    }


    #endregion

    #region Doctors Management

    [HttpGet]
    public async Task<IActionResult> Doctors()
    {
        ViewData["ActivePage"] = "Doctors";

        var doctors = await apiFacade.Doctors.GetAllAsync();
        var procedures = await apiFacade.Procedures.GetAllAsync();

        return View(new DoctorsViewModel
        {
            Doctors = doctors,
            Procedures = procedures,
            SuccessMessage = TempData["SuccessMessage"] as string,
            ErrorMessage = TempData["ErrorMessage"] as string
        });
    }

    [HttpGet]
    public async Task<IActionResult> CreateDoctor()
    {
        ViewData["ActivePage"] = "Doctors";

        var procedures = await apiFacade.Procedures.GetAllAsync();

        return View(new CreateDoctorViewModel
        {
            AllProcedures = procedures
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor(CreateDoctorViewModel model)
    {
        ViewData["ActivePage"] = "Doctors";

        if (!ModelState.IsValid)
        {
            model.AllProcedures = await apiFacade.Procedures.GetAllAsync();
            return View(model);
        }

        var response = await apiFacade.Doctors.CreateAsync(
            model.FirstName,
            model.LastName,
            model.Type,
            model.SelectedProcedureIds);

        TempData[response != null ? "SuccessMessage" : "ErrorMessage"] =
            response?.Message ?? "Failed to create doctor.";

        return RedirectToAction(nameof(Doctors));
    }

    [HttpGet]
    public async Task<IActionResult> EditDoctor(int id)
    {
        ViewData["ActivePage"] = "Doctors";

        var doctor = await apiFacade.Doctors.GetByIdAsync(id);
        if (doctor == null)
        {
            TempData["ErrorMessage"] = "Doctor not found.";
            return RedirectToAction(nameof(Doctors));
        }

        var procedures = await apiFacade.Procedures.GetAllAsync();

        return View(new EditDoctorViewModel
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Type = doctor.Specialty,
            AllProcedures = procedures
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditDoctor(EditDoctorViewModel model)
    {
        ViewData["ActivePage"] = "Doctors";

        if (!ModelState.IsValid)
        {
            model.AllProcedures = await apiFacade.Procedures.GetAllAsync();
            return View(model);
        }

        var response = await apiFacade.Doctors.UpdateAsync(
            model.Id,
            model.FirstName,
            model.LastName,
            model.Type,
            model.SelectedProcedureIds);

        TempData[response != null ? "SuccessMessage" : "ErrorMessage"] =
            response?.Message ?? "Failed to update doctor.";

        return RedirectToAction(nameof(Doctors));
    }

    [HttpPost]
    public async Task<IActionResult> CloneDoctor(int id, string newFirstName, string newLastName)
    {
        var response = await apiFacade.Doctors.CloneAsync(id, newFirstName, newLastName);

        TempData[response?.Data != null ? "SuccessMessage" : "ErrorMessage"] =
            response?.Message ?? "Failed to clone doctor.";

        return RedirectToAction(nameof(Doctors));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var response = await apiFacade.Doctors.DeleteAsync(id);

        TempData[response != null ? "SuccessMessage" : "ErrorMessage"] =
            response?.Message ?? "Doctor not found or could not be deleted.";

        return RedirectToAction(nameof(Doctors));
    }

    #endregion

    #region Procedures Management

    [HttpGet]
    public async Task<IActionResult> Procedures()
    {
        ViewData["ActivePage"] = "Procedures";
        var procedures = await apiFacade.Procedures.GetAllAsync();

        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        ViewBag.ErrorMessage = TempData["ErrorMessage"];

        return View(procedures);
    }

    [HttpGet]
    public IActionResult CreateProcedure()
    {
        ViewData["ActivePage"] = "Procedures";
        return View(new CreateProcedureViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateProcedure(CreateProcedureViewModel model)
    {
        ViewData["ActivePage"] = "Procedures";

        if (!ModelState.IsValid)
            return View(model);

        var response = await apiFacade.Procedures.CreateAsync(
            model.Code, model.Name, model.DurationMinutes, model.Price);

        TempData[response != null ? "SuccessMessage" : "ErrorMessage"] =
            response?.Message ?? "Failed to create procedure.";

        return RedirectToAction(nameof(Procedures));
    }

    [HttpGet]
    public async Task<IActionResult> EditProcedure(int id)
    {
        ViewData["ActivePage"] = "Procedures";

        var procedure = await apiFacade.Procedures.GetByIdAsync(id);
        if (procedure == null)
        {
            TempData["ErrorMessage"] = "Procedure not found.";
            return RedirectToAction(nameof(Procedures));
        }

        return View(new EditProcedureViewModel
        {
            Id = procedure.Id,
            Code = procedure.Code,
            Name = procedure.Name,
            DurationMinutes = procedure.DurationMinutes,
            Price = procedure.Price
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditProcedure(EditProcedureViewModel model)
    {
        ViewData["ActivePage"] = "Procedures";

        if (!ModelState.IsValid)
            return View(model);

        var response = await apiFacade.Procedures.UpdateAsync(
            model.Id, model.Code, model.Name, model.DurationMinutes, model.Price);

        TempData[response != null ? "SuccessMessage" : "ErrorMessage"] =
            response?.Message ?? "Failed to update procedure.";

        return RedirectToAction(nameof(Procedures));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProcedure(int id)
    {
        var response = await apiFacade.Procedures.DeleteAsync(id);

        TempData[response != null ? "SuccessMessage" : "ErrorMessage"] =
            response?.Message ?? "Procedure not found or could not be deleted.";

        return RedirectToAction(nameof(Procedures));
    }

    #endregion

    [GeneratedRegex(@"#(\d+)")]
    private static partial Regex ProcedureIdRegex();
}
