using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public class CreateDoctorHandler(IDoctorRepository doctorRepo) : IRequestHandler<CreateDoctorCommand, int>
{
    private readonly IDoctorRepository _doctorRepo = doctorRepo;

    public async Task<int> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        if (request.SelectedProcedureIds.Count > 3)
            throw new InvalidOperationException("A doctor can perform maximum 3 procedures.");

        var doctor = new Doctor
        {
            FullName = request.FullName,
            DoctorProcedures = [.. request.SelectedProcedureIds.Select(pid => new DoctorProcedure { ProcedureId = pid })]
        };

        await _doctorRepo.AddAsync(doctor, cancellationToken);
        await _doctorRepo.SaveChangesAsync(cancellationToken);

        return doctor.Id;
    }
}
