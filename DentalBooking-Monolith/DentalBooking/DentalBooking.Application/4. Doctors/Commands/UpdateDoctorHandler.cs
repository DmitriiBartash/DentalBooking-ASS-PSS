using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public class UpdateDoctorHandler(IDoctorRepository doctorRepo) : IRequestHandler<UpdateDoctorCommand, bool>
{
    private readonly IDoctorRepository _doctorRepo = doctorRepo;

    public async Task<bool> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null) return false;

        doctor.FullName = request.FullName;

        doctor.DoctorProcedures.Clear();
        foreach (var pid in request.SelectedProcedureIds.Take(3)) 
        {
            doctor.DoctorProcedures.Add(new DoctorProcedure
            {
                DoctorId = doctor.Id,
                ProcedureId = pid
            });
        }

        await _doctorRepo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
