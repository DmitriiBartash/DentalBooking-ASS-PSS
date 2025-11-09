using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using MediatR;

namespace DentalBooking.Application.Doctors.Queries;

public class GetAvailableDoctorsHandler(
    IDoctorRepository doctorRepo,
    IProcedureRepository procedureRepo,
    IBookingRepository bookingRepo) : IRequestHandler<GetAvailableDoctorsQuery, List<Doctor>>
{
    private readonly IDoctorRepository _doctorRepo = doctorRepo;
    private readonly IProcedureRepository _procedureRepo = procedureRepo;
    private readonly IBookingRepository _bookingRepo = bookingRepo;

    public async Task<List<Doctor>> Handle(GetAvailableDoctorsQuery request, CancellationToken cancellationToken)
    {
        // 1. Validate procedure
        var procedure = await _procedureRepo.GetByIdAsync(request.ProcedureId, cancellationToken) ?? throw new InvalidOperationException("Procedure not found.");

        var endUtc = request.StartUtc.Add(procedure.Duration);

        // 2. Find doctors who can perform this procedure
        var allDoctors = await _doctorRepo.GetAllAsync(cancellationToken);
        var suitableDoctors = allDoctors
            .Where(d => d.DoctorProcedures.Any(dp => dp.ProcedureId == request.ProcedureId))
            .ToList();

        // 3. Filter by availability
        var availableDoctors = new List<Doctor>();
        foreach (var doctor in suitableDoctors)
        {
            var hasConflict = await _bookingRepo.HasConflictAsync(doctor.Id, request.StartUtc, endUtc, cancellationToken);
            if (!hasConflict)
                availableDoctors.Add(doctor);
        }

        return availableDoctors;
    }
}
