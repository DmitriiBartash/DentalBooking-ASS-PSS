using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Application.Doctors.DTO;
using MediatR;

namespace DentalBooking.Application.Doctors.Queries;

public class GetDoctorByIdHandler(IDoctorRepository doctorRepo)
    : IRequestHandler<GetDoctorByIdQuery, DoctorDto?>
{
    private readonly IDoctorRepository _doctorRepo = doctorRepo;

    public async Task<DoctorDto?> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepo.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null) return null;

        return new DoctorDto(
            doctor.Id,
            doctor.FullName,
            [.. doctor.DoctorProcedures.Select(dp => dp.ProcedureId)]
        );
    }
}
