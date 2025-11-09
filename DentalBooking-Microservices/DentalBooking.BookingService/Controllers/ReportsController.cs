using DentalBooking.BookingService.Application.Dtos;
using DentalBooking.BookingService.Infrastructure.Persistence;
using DentalBooking.BookingService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(DataContext context, ILogger<ReportsController> logger) : ControllerBase
{
    private readonly DataContext _context = context;
    private readonly ILogger<ReportsController> _logger = logger;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] int? doctorId,
        [FromQuery] string? clientId,
        [FromQuery] int? procedureId,   
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        _logger.LogInformation(
            "Generating report for doctor={DoctorId}, procedure={ProcedureId}, client={ClientId}, from={From}, to={To}",
            doctorId, procedureId, clientId, from, to);

        var query = _context.Bookings.AsQueryable();

        if (doctorId.HasValue)
            query = query.Where(b => b.DoctorId == doctorId.Value);

        if (procedureId.HasValue)  
            query = query.Where(b => b.ProcedureId == procedureId.Value);

        if (!string.IsNullOrEmpty(clientId))
            query = query.Where(b => b.ClientId == clientId);

        if (from.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
            query = query.Where(b => b.StartUtc >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
            query = query.Where(b => b.StartUtc <= toUtc);
        }

        var bookings = await query.ToListAsync();

        if (bookings.Count == 0)
        {
            _logger.LogInformation("No bookings found for selected filters");
            return Ok(new BookingReportDto());
        }

        var report = new BookingReportDto
        {
            TotalBookings = bookings.Count,
            Completed = bookings.Count(b => b.Status == BookingStatus.Completed),
            Cancelled = bookings.Count(b => b.Status == BookingStatus.Cancelled),
            ByProcedure = bookings
                .GroupBy(b => $"Procedure #{b.ProcedureId}")
                .ToDictionary(g => g.Key, g => g.Count())
        };

        _logger.LogInformation("Report generated successfully with {Count} entries", report.TotalBookings);
        return Ok(report);
    }
}
