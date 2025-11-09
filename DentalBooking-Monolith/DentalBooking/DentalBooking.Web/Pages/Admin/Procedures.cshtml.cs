using DentalBooking.Application.Procedures.DTO;
using DentalBooking.Application.Procedures.Queries;
using DentalBooking.Application.Procedures.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Admin;

public class ProceduresModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    public List<ProcedureDto> Procedures { get; set; } = [];

    [TempData] public string? SuccessMessage { get; set; }
    [TempData] public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            ViewData["ActivePage"] = "Procedures";
            Procedures = await _mediator.Send(new GetProceduresQuery());
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load procedures: {ex.Message}";
            Procedures = [];
        }
    }

    public IActionResult OnGetCreate() =>
        RedirectToPage("/Admin/CreateProcedure");

    public IActionResult OnGetEdit(int id) =>
        RedirectToPage("/Admin/EditProcedure", new { id });

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var deleted = await _mediator.Send(new DeleteProcedureCommand(id));

            if (!deleted)
                ErrorMessage = "Procedure not found.";
            else
                SuccessMessage = "Procedure deleted successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete procedure: {ex.Message}";
        }

        return RedirectToPage();
    }
}
