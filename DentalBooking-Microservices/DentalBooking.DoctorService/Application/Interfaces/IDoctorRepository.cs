using DentalBooking.DoctorService.Domain.Entities;

namespace DentalBooking.DoctorService.Application.Interfaces;

public interface IDoctorRepository
{
    Task<List<Doctor>> GetAllAsync();
    Task<Doctor?> GetByIdAsync(int id);
    Task AddAsync(Doctor doctor);
    Task DeleteAsync(Doctor doctor);
    Task<int> SaveChangesAsync();
    Task<List<Doctor>> GetByProcedureAsync(int procedureId);
}
