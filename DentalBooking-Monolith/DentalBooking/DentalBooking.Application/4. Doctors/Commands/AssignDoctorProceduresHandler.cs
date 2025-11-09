using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public class AssignDoctorProceduresHandler( IDoctorRepository doctorRepo, IProcedureRepository procedureRepo) : IRequestHandler<AssignDoctorProceduresCommand, bool>
{
    private readonly IDoctorRepository _doctorRepo = doctorRepo;
    private readonly IProcedureRepository _procedureRepo = procedureRepo;

    public async Task<bool> Handle(AssignDoctorProceduresCommand request, CancellationToken cancellationToken)
    {
        if (request.ProcedureIds.Count > 3)
            throw new InvalidOperationException("A doctor cannot have more than 3 procedures assigned.");

        var doctor = await _doctorRepo.GetByIdAsync(request.DoctorId, cancellationToken)
                     ?? throw new InvalidOperationException("Doctor not found.");

        foreach (var procedureId in request.ProcedureIds)
        {
            _ = await _procedureRepo.GetByIdAsync(procedureId, cancellationToken)
                ?? throw new InvalidOperationException($"Procedure with ID {procedureId} does not exist.");
        }

        doctor.DoctorProcedures.Clear();
        foreach (var procedureId in request.ProcedureIds)
        {
            doctor.DoctorProcedures.Add(new DoctorProcedure
            {
                DoctorId = doctor.Id,
                ProcedureId = procedureId
            });
        }

        await _doctorRepo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
