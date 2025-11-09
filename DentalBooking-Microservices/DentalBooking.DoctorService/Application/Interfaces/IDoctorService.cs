using DentalBooking.DoctorService.Application.Dtos;

namespace DentalBooking.DoctorService.Application.Interfaces;

public interface IDoctorService
{
    Task<List<DoctorDto>> GetAllAsync();
    Task<DoctorDto?> GetByIdAsync(int id);

    Task<DoctorDto> CreateAsync(CreateDoctorRequest request);
    Task<bool> UpdateAsync(int id, UpdateDoctorRequest request);
    Task<bool> DeleteAsync(int id);

    Task<bool> AssignProceduresAsync(int doctorId, List<int> procedureIds);
    Task<bool> RemoveProceduresAsync(int doctorId, List<int> procedureIds);

    Task<DoctorDto> CloneAsync(int doctorId, string newFirstName, string newLastName);

    Task<List<DoctorDto>> GetByProcedureAsync(int procedureId);
}
