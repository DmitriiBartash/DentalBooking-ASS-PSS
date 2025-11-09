using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Application.Doctors.DTO;
using MediatR;

namespace DentalBooking.Application.Doctors.Queries;

public class GetDoctorsQueryHandler(IDoctorRepository repo)
    : IRequestHandler<GetDoctorsQuery, List<DoctorDto>>
{
    private readonly IDoctorRepository _repo = repo;

    public async Task<List<DoctorDto>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _repo.GetAllAsync(cancellationToken);

        return [.. doctors
            .Select(d => new DoctorDto(
                d.Id,
                d.FullName,
                [.. d.DoctorProcedures.Select(dp => dp.ProcedureId)]
            ))];
    }
}
