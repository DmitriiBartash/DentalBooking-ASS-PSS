using System.ComponentModel.DataAnnotations;
using DentalBooking.Application.Doctors.Commands;
using DentalBooking.Application.Procedures.DTO;
using DentalBooking.Application.Procedures.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Admin;

public class CreateDoctorModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<ProcedureDto> AllProcedures { get; set; } = [];

    [TempData] public string? ErrorMessage { get; set; }
    [TempData] public string? SuccessMessage { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        public List<int> SelectedProcedureIds { get; set; } = [];
    }

    public async Task OnGetAsync()
    {
        ViewData["ActivePage"] = "Doctors";
        AllProcedures = await _mediator.Send(new GetProceduresQuery());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ViewData["ActivePage"] = "Doctors";
        AllProcedures = await _mediator.Send(new GetProceduresQuery());

        Input.SelectedProcedureIds = Input.SelectedProcedureIds?.Distinct().ToList() ?? [];

        if (Input.SelectedProcedureIds.Count == 0)
            ModelState.AddModelError("Input.SelectedProcedureIds", "Please select at least one procedure.");
        else if (Input.SelectedProcedureIds.Count > 3)
            ModelState.AddModelError("Input.SelectedProcedureIds", "You can select up to 3 procedures only.");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _mediator.Send(new CreateDoctorCommand(
                Input.FullName,
                Input.SelectedProcedureIds
            ));

            SuccessMessage = "Doctor created successfully!";
            return RedirectToPage("/Admin/Doctors");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
