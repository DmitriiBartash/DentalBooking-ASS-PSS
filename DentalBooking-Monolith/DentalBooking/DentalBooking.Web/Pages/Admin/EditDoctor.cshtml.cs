using System.ComponentModel.DataAnnotations;
using DentalBooking.Application.Doctors.Commands;
using DentalBooking.Application.Doctors.Queries;
using DentalBooking.Application.Procedures.DTO;
using DentalBooking.Application.Procedures.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Admin;

public class EditDoctorModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<ProcedureDto> AllProcedures { get; set; } = [];

    [TempData] public string? ErrorMessage { get; set; }
    [TempData] public string? SuccessMessage { get; set; }

    public class InputModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "Select at least 1 procedure.")]
        [MaxLength(3, ErrorMessage = "You can select maximum 3 procedures.")]
        public List<int> SelectedProcedureIds { get; set; } = [];
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var doctor = await _mediator.Send(new GetDoctorByIdQuery(id));
        if (doctor == null)
        {
            ErrorMessage = "Doctor not found.";
            return RedirectToPage("/Admin/Doctors");
        }

        AllProcedures = await _mediator.Send(new GetProceduresQuery());

        Input = new InputModel
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            SelectedProcedureIds = doctor.ProcedureIds
        };

        ViewData["ActivePage"] = "Doctors";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AllProcedures = await _mediator.Send(new GetProceduresQuery());
        ViewData["ActivePage"] = "Doctors";

        if (!ModelState.IsValid)
            return Page();

        if (Input.SelectedProcedureIds.Any(id => !AllProcedures.Any(p => p.Id == id)))
        {
            ErrorMessage = "Invalid procedure selection.";
            return Page();
        }

        try
        {
            var updated = await _mediator.Send(new UpdateDoctorCommand(
                Input.Id,
                Input.FullName,
                Input.SelectedProcedureIds
            ));

            if (!updated)
            {
                ErrorMessage = "Doctor not found.";
                return RedirectToPage("/Admin/Doctors");
            }

            SuccessMessage = "Doctor updated successfully!";
            return RedirectToPage("/Admin/Doctors");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
