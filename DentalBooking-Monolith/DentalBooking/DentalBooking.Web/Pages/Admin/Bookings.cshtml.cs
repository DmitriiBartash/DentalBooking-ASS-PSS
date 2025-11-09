using DentalBooking.Application.Bookings.DTO;
using DentalBooking.Application.Bookings.Queries;
using DentalBooking.Application.Doctors.DTO;
using DentalBooking.Application.Doctors.Queries;
using DentalBooking.Application.Procedures.DTO;
using DentalBooking.Application.Procedures.Queries;
using DentalBooking.Application.Bookings.Commands;
using DentalBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Admin;

public class BookingsModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    public List<BookingDto> Bookings { get; set; } = [];
    public List<DoctorDto> AllDoctors { get; set; } = [];
    public List<ProcedureDto> AllProcedures { get; set; } = [];

    [BindProperty(SupportsGet = true)] public int? DoctorId { get; set; }
    [BindProperty(SupportsGet = true)] public int? ProcedureId { get; set; }
    [BindProperty(SupportsGet = true)] public BookingStatus? Status { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? From { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? To { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        ViewData["ActivePage"] = "Bookings";

        await LoadFiltersAsync();

        Bookings = await _mediator.Send(new GetBookingsQuery(
            ClientId: null,
            DoctorId: DoctorId,
            ProcedureId: ProcedureId,
            FromUtc: From?.ToUniversalTime(),
            ToUtc: To?.ToUniversalTime(),
            Status: Status,
            SortBy: "date",
            Page: 1,
            PageSize: 50
        ));

        if (TempData.ContainsKey("ErrorMessage"))
            ErrorMessage = TempData["ErrorMessage"]?.ToString();
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(int id, BookingStatus newStatus)
    {
        try
        {
            await _mediator.Send(new UpdateBookingStatusCommand(id, newStatus));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new
        {
            DoctorId,
            ProcedureId,
            Status,
            From,
            To
        });
    }

    private async Task LoadFiltersAsync()
    {
        AllDoctors = await _mediator.Send(new GetDoctorsQuery());
        AllProcedures = await _mediator.Send(new GetProceduresQuery());
    }
}
