using DentalBooking.Application.Bookings.Commands;
using DentalBooking.Application.Doctors.Queries;
using DentalBooking.Application.Procedures.Queries;
using DentalBooking.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DentalBooking.Web.Pages.Client;

public class CreateBookingModel(
    IMediator mediator,
    UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly IMediator _mediator = mediator;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList Procedures { get; set; } = default!;
    public SelectList AvailableDoctors { get; set; } = new SelectList(Enumerable.Empty<object>());

    [TempData] public string? SuccessMessage { get; set; }
    [TempData] public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        public int ProcedureId { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        [Required]
        public int DoctorId { get; set; }
    }

    public async Task OnGetAsync()
    {
        ViewData["ActivePage"] = "CreateBooking";
        await LoadProceduresAsync();
    }

    private async Task LoadProceduresAsync()
    {
        var procedures = await _mediator.Send(new GetProceduresQuery());
        Procedures = new SelectList(procedures, "Id", "Name");
    }

    public async Task<JsonResult> OnGetAvailableDoctorsAsync(int procedureId, DateTime start)
    {
        var doctors = await _mediator.Send(new GetAvailableDoctorsQuery(procedureId, start));
        return new JsonResult(doctors.Select(d => new { d.Id, d.FullName }));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ViewData["ActivePage"] = "CreateBooking";

        if (!ModelState.IsValid)
        {
            await LoadProceduresAsync();
            return Page();
        }

        var clientId = _userManager.GetUserId(User);
        if (clientId == null)
        {
            return RedirectToPage("/Login");
        }

        var startLocal = Input.Date.Date + Input.Time;
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal);

        if (startUtc <= DateTime.UtcNow)
        {
            ErrorMessage = "Booking must be in the future.";
            await LoadProceduresAsync();
            return Page();
        }

        try
        {
            var id = await _mediator.Send(new CreateBookingCommand(
                clientId,
                Input.DoctorId,
                Input.ProcedureId,
                startUtc));

            SuccessMessage = $"Booking #{id} successfully created!";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await LoadProceduresAsync();
            return Page();
        }
    }
}
