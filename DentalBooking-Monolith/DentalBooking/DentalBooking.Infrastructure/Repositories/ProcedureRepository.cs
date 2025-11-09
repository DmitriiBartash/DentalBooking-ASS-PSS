using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Domain.Entities;
using DentalBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DentalBooking.Infrastructure.Repositories;

public class ProcedureRepository(AppDbContext context) : IProcedureRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Procedure?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Procedures.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Procedure>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Procedures.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Procedure procedure, CancellationToken cancellationToken)
    {
        await _context.Procedures.AddAsync(procedure, cancellationToken);
    }

    public Task DeleteAsync(Procedure procedure, CancellationToken cancellationToken)
    {
        _context.Procedures.Remove(procedure);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
