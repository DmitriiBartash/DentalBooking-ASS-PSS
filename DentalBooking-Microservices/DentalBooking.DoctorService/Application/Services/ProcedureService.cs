using DentalBooking.DoctorService.Application.Dtos;
using DentalBooking.DoctorService.Application.Interfaces;
using DentalBooking.DoctorService.Domain.Entities;

namespace DentalBooking.DoctorService.Application.Services;

public class ProcedureService(IProcedureRepository repo) : IProcedureService
{
    public async Task<List<ProcedureDto>> GetAllAsync()
    {
        var list = await repo.GetAllAsync();

        return [.. list.Select(p => new ProcedureDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            DurationMinutes = (int)p.Duration.TotalMinutes,
            Price = p.Price
        })];
    }

    public async Task<ProcedureDto?> GetByIdAsync(int id)
    {
        var p = await repo.GetByIdAsync(id);
        if (p == null) return null;

        return new ProcedureDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            DurationMinutes = (int)p.Duration.TotalMinutes,
            Price = p.Price
        };
    }

    public async Task<int> CreateAsync(CreateProcedureRequest request)
    {
        var entity = new Procedure
        {
            Code = request.Code,
            Name = request.Name,
            Duration = TimeSpan.FromMinutes(request.DurationMinutes),
            Price = request.Price
        };

        await repo.AddAsync(entity);
        await repo.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, UpdateProcedureRequest request)
    {
        var entity = await repo.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Duration = TimeSpan.FromMinutes(request.DurationMinutes);
        entity.Price = request.Price;

        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await repo.GetByIdAsync(id);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        await repo.SaveChangesAsync();
        return true;
    }
}
