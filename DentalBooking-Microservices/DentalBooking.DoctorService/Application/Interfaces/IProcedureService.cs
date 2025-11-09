using DentalBooking.DoctorService.Application.Dtos;

namespace DentalBooking.DoctorService.Application.Interfaces;

public interface IProcedureService
{
    Task<List<ProcedureDto>> GetAllAsync();
    Task<ProcedureDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateProcedureRequest request);
    Task<bool> UpdateAsync(int id, UpdateProcedureRequest request);
    Task<bool> DeleteAsync(int id);
}
