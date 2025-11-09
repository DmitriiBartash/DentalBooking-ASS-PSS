using System.ComponentModel.DataAnnotations;
using DentalBooking.Application.Procedures.Commands;
using DentalBooking.Application.Procedures.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Admin;

public class EditProcedureModel(IMediator mediator, ILogger<EditProcedureModel> logger) : PageModel
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<EditProcedureModel> _logger = logger;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData] public string? ErrorMessage { get; set; }
    [TempData] public string? SuccessMessage { get; set; }

    public class InputModel
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 300)]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var proc = await _mediator.Send(new GetProcedureByIdQuery(id));
        if (proc == null)
        {
            ErrorMessage = "Procedure not found.";
            return RedirectToPage("/Admin/Procedures");
        }

        Input = new InputModel
        {
            Id = proc.Id,
            Code = proc.Code,
            Name = proc.Name,
            DurationMinutes = (int)proc.DurationMinutes,
            Price = proc.Price
        };

        ViewData["ActivePage"] = "Procedures";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["ActivePage"] = "Procedures";
            return Page();
        }

        try
        {
            var updated = await _mediator.Send(new UpdateProcedureCommand(
                Input.Id,
                Input.Code,
                Input.Name,
                TimeSpan.FromMinutes(Input.DurationMinutes),
                Input.Price
            ));

            if (!updated)
            {
                ErrorMessage = "Procedure not found.";
                return RedirectToPage("/Admin/Procedures");
            }

            SuccessMessage = "Procedure updated successfully!";
            return RedirectToPage("/Admin/Procedures");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating procedure with Id={Id}", Input.Id);
            ErrorMessage = "Unexpected error while updating procedure.";
            ViewData["ActivePage"] = "Procedures";
            return Page();
        }
    }
}
