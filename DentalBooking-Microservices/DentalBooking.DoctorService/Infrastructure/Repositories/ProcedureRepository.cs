using DentalBooking.DoctorService.Application.Interfaces;
using DentalBooking.DoctorService.Domain.Entities;
using DentalBooking.DoctorService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.DoctorService.Infrastructure.Repositories;

public class ProcedureRepository(DoctorDbContext ctx) : IProcedureRepository
{
    private readonly DoctorDbContext _ctx = ctx;

    public async Task<List<Procedure>> GetAllAsync() => await _ctx.Procedures.AsNoTracking().ToListAsync();

    public async Task<Procedure?> GetByIdAsync(int id) => await _ctx.Procedures.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Procedure entity) => await _ctx.Procedures.AddAsync(entity);

    public async Task DeleteAsync(Procedure entity)
    {
        _ctx.Procedures.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync() => await _ctx.SaveChangesAsync();
}
