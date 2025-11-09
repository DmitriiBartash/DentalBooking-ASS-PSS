using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Application.Doctors.DTO;
using DentalBooking.Domain.Entities;
using DentalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.Infrastructure.Repositories;

public class DoctorRepository(AppDbContext context) : IDoctorRepository
{
    private readonly AppDbContext _context = context;

    public async Task<bool> CanPerformProcedureAsync(int doctorId, int procedureId, CancellationToken cancellationToken)
    {
        return await _context.DoctorProcedures
            .AnyAsync(dp => dp.DoctorId == doctorId && dp.ProcedureId == procedureId, cancellationToken);
    }

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Doctors
            .Include(d => d.DoctorProcedures)
            .ThenInclude(dp => dp.Procedure)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<List<Doctor>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Doctors
            .Include(d => d.DoctorProcedures)
            .ThenInclude(dp => dp.Procedure)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        await _context.Doctors.AddAsync(doctor, cancellationToken);
    }

    public Task DeleteAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        _context.Doctors.Remove(doctor);
        return Task.CompletedTask;
    }


    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<DoctorDto>> GetAllProjectedAsync(CancellationToken cancellationToken)
    {
        return await _context.Doctors
            .Include(d => d.DoctorProcedures)
            .Select(d => new DoctorDto(
                d.Id,
                d.FullName,
                d.DoctorProcedures.Select(dp => dp.ProcedureId).ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}
