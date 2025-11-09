using DentalBooking.Application.Doctors.DTO;
using DentalBooking.Domain.Entities;

namespace DentalBooking.Application.Common.Interfaces;

public interface IDoctorRepository
{
    Task<bool> CanPerformProcedureAsync(int doctorId, int procedureId, CancellationToken cancellationToken);
    Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<Doctor>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Doctor doctor, CancellationToken cancellationToken);
    Task DeleteAsync(Doctor doctor, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<List<DoctorDto>> GetAllProjectedAsync(CancellationToken cancellationToken);
}
