using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using DentalBooking.Domain.Enums;
using MediatR;

namespace DentalBooking.Application.Bookings.Commands;

public class CreateBookingHandler(
    IBookingRepository bookingRepo,
    IDoctorRepository doctorRepo,
    IProcedureRepository procedureRepo,
    IUserService userService,
    IEmailSender emailSender) : IRequestHandler<CreateBookingCommand, int>
{
    private readonly IBookingRepository _bookingRepo = bookingRepo;
    private readonly IDoctorRepository _doctorRepo = doctorRepo;
    private readonly IProcedureRepository _procedureRepo = procedureRepo;
    private readonly IUserService _userService = userService;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (!await _userService.IsEmailConfirmedAsync(request.ClientId))
            throw new InvalidOperationException("Email must be confirmed before booking.");

        if (!await _doctorRepo.CanPerformProcedureAsync(request.DoctorId, request.ProcedureId, cancellationToken))
            throw new InvalidOperationException("Doctor cannot perform this procedure.");

        var procedure = await _procedureRepo.GetByIdAsync(request.ProcedureId, cancellationToken)
                        ?? throw new InvalidOperationException("Procedure not found.");

        var endUtc = request.StartUtc.Add(procedure.Duration);

        if (await _bookingRepo.HasConflictAsync(request.DoctorId, request.StartUtc, endUtc, cancellationToken))
            throw new InvalidOperationException("Doctor is not available at this time.");

        var booking = new Booking
        {
            ClientId = request.ClientId,
            DoctorId = request.DoctorId,
            ProcedureId = request.ProcedureId,
            StartUtc = request.StartUtc,
            EndUtc = endUtc,
            Status = BookingStatus.Confirmed
        };

        await _bookingRepo.AddAsync(booking, cancellationToken);

        if (await _bookingRepo.HasConflictAsync(request.DoctorId, request.StartUtc, endUtc, cancellationToken))
            throw new InvalidOperationException("Doctor is not available anymore, please pick another slot.");

        await _bookingRepo.SaveChangesAsync(cancellationToken);

        var clientEmail = await _userService.GetEmailAsync(request.ClientId);
        await _emailSender.SendEmailAsync(
            clientEmail,
            "Booking confirmation",
            $"Your booking for {procedure.Name} on {request.StartUtc:dd.MM.yyyy HH:mm} is confirmed."
        );

        return booking.Id;
    }
}
