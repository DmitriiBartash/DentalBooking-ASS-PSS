using DentalBooking.Application.Reports.DTO;
using DentalBooking.Application.Reports.Queries;
using DentalBooking.Application.Doctors.Queries;
using DentalBooking.Application.Procedures.Queries;
using DentalBooking.Application.Doctors.DTO;
using DentalBooking.Application.Procedures.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Admin;

public class ReportsModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    public BookingStatisticsDto? Stats { get; private set; }
    public List<DoctorDto> Doctors { get; private set; } = [];
    public List<ProcedureDto> Procedures { get; private set; } = [];

    [BindProperty(SupportsGet = true)] public int? DoctorId { get; set; }
    [BindProperty(SupportsGet = true)] public int? ProcedureId { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? FromDate { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? ToDate { get; set; }

    [TempData] public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            ViewData["ActivePage"] = "Reports"; 

            Doctors = await _mediator.Send(new GetDoctorsQuery());
            Procedures = await _mediator.Send(new GetProceduresQuery());

            Stats = await _mediator.Send(new GetStatisticsQuery(
                DoctorId,
                ProcedureId,
                FromDate?.ToUniversalTime(),
                ToDate?.ToUniversalTime()
            ));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load reports: {ex.Message}";
            Stats = null;
        }
    }
}
