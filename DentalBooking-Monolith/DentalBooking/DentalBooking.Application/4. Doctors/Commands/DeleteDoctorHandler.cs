using DentalBooking.Application.Common.Interfaces;
using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public class DeleteDoctorHandler(IDoctorRepository doctorRepo) : IRequestHandler<DeleteDoctorCommand, bool>
{
    private readonly IDoctorRepository _doctorRepo = doctorRepo;

    public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null) return false;

        await _doctorRepo.DeleteAsync(doctor, cancellationToken);
        await _doctorRepo.SaveChangesAsync(cancellationToken);
        return true;
    }
}
