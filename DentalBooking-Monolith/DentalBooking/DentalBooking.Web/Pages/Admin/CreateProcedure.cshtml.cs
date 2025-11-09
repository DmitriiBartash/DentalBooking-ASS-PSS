using System.ComponentModel.DataAnnotations;
using DentalBooking.Application.Procedures.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Admin;

public class CreateProcedureModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData] public string? ErrorMessage { get; set; }
    [TempData] public string? SuccessMessage { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes.")]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000 MDL.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }

    public void OnGet()
    {
        ViewData["ActivePage"] = "Procedures";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ViewData["ActivePage"] = "Procedures";

        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _mediator.Send(new CreateProcedureCommand(
                Input.Code,
                Input.Name,
                TimeSpan.FromMinutes(Input.DurationMinutes),
                Input.Price
            ));

            SuccessMessage = "Procedure created successfully!";
            return RedirectToPage("/Admin/Procedures");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
