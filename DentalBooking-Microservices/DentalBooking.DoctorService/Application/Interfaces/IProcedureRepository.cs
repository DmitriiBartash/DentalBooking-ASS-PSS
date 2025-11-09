using DentalBooking.DoctorService.Domain.Entities;

namespace DentalBooking.DoctorService.Application.Interfaces;

public interface IProcedureRepository
{
    Task<List<Procedure>> GetAllAsync();
    Task<Procedure?> GetByIdAsync(int id);
    Task AddAsync(Procedure entity);
    Task DeleteAsync(Procedure entity);
    Task<int> SaveChangesAsync();
}
