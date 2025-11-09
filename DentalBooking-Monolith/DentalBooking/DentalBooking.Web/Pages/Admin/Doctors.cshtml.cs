using DentalBooking.Application.Doctors.DTO;
using DentalBooking.Application.Doctors.Queries;
using DentalBooking.Application.Doctors.Commands;
using DentalBooking.Application.Procedures.DTO;
using DentalBooking.Application.Procedures.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;

namespace DentalBooking.Web.Pages.Admin;

public class DoctorsModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    public List<DoctorDto> Doctors { get; set; } = [];
    public List<ProcedureDto> AllProcedures { get; set; } = [];

    [TempData] public string? SuccessMessage { get; set; }
    [TempData] public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Doctors = await _mediator.Send(new GetDoctorsQuery());
        AllProcedures = await _mediator.Send(new GetProceduresQuery());

        ViewData["ActivePage"] = "Doctors";
    }

    public IActionResult OnGetCreate() => RedirectToPage("/Admin/CreateDoctor");

    public IActionResult OnGetEdit(int id) => RedirectToPage("/Admin/EditDoctor", new { id });

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var deleted = await _mediator.Send(new DeleteDoctorCommand(id));

        if (!deleted)
            ErrorMessage = "Doctor not found.";
        else
            SuccessMessage = "Doctor deleted successfully.";

        return RedirectToPage();
    }
}
