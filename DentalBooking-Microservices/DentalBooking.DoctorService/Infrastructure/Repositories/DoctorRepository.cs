using DentalBooking.DoctorService.Application.Interfaces;
using DentalBooking.DoctorService.Domain.Entities;
using DentalBooking.DoctorService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.DoctorService.Infrastructure.Repositories;

public class DoctorRepository(DoctorDbContext ctx) : IDoctorRepository
{
    private readonly DoctorDbContext _ctx = ctx;

    public async Task<List<Doctor>> GetAllAsync()
    {
        return await _ctx.Doctors
            .Include(d => d.DoctorProcedures)
            .ThenInclude(dp => dp.Procedure)
            .ToListAsync();
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _ctx.Doctors
            .Include(d => d.DoctorProcedures)
            .ThenInclude(dp => dp.Procedure)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddAsync(Doctor doctor)
    {
        await _ctx.Doctors.AddAsync(doctor);
    }

    public async Task DeleteAsync(Doctor doctor)
    {
        _ctx.Doctors.Remove(doctor);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _ctx.SaveChangesAsync();
    }

    public async Task<List<Doctor>> GetByProcedureAsync(int procedureId)
    {
        return await _ctx.Doctors
            .Include(d => d.DoctorProcedures)
            .ThenInclude(dp => dp.Procedure)
            .Where(d => d.DoctorProcedures.Any(dp => dp.ProcedureId == procedureId))
            .ToListAsync();
    }
}
