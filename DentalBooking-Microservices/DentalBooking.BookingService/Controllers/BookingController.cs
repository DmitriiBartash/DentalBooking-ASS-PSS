using DentalBooking.BookingService.Application.Commands;
using DentalBooking.BookingService.Application.Mediator;
using DentalBooking.BookingService.Application.Dtos;
using DentalBooking.BookingService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace DentalBooking.BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController(IMediator mediator, IBookingRepository repository, ILogger<BookingController> logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IBookingRepository _repository = repository;
    private readonly ILogger<BookingController> _logger = logger;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
    {
        try
        {
            var bookingId = await _mediator.SendAsync<int>(command);
            _logger.LogInformation(
                "Booking {BookingId} created for DoctorId={DoctorId}, ClientId={ClientId}",
                bookingId, command.DoctorId, command.ClientId);

            return Ok(new
            {
                data = bookingId,
                message = "Booking created successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating booking");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("cancel/{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _mediator.SendAsync(new CancelBookingCommand(id));
            _logger.LogInformation("Booking {BookingId} cancelled", id);

            return Ok(new { message = "Booking cancelled successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while cancelling booking {BookingId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _repository.GetByIdAsync(id);
        if (booking is null)
            return NotFound(new { message = "Booking not found." });

        return Ok(booking);
    }

    [HttpPost("busy")]
    public async Task<IActionResult> GetBusyDoctors([FromBody] BusyDoctorsRequest request)
    {
        if (request.DoctorIds is null || request.DoctorIds.Count == 0)
            return BadRequest("Doctor list is empty.");

        var busyDoctors = await _repository.GetBusyDoctorsAsync(request.DoctorIds, request.StartUtc);
        return Ok(busyDoctors);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] int? doctorId,
    [FromQuery] string? clientId,
    [FromQuery] int? procedureId,
    [FromQuery] string? status,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
    {
        var bookings = await _repository.GetAllAsync(doctorId, clientId, procedureId, status, from, to);

        if (bookings == null || bookings.Count == 0)
            return NotFound(new { message = "No bookings found." });

        return Ok(bookings.Select(b => new
        {
            b.Id,
            b.DoctorId,
            b.ClientId,
            b.ProcedureId,
            b.StartUtc,
            Status = b.Status.ToString() 
        }));
    }
}
