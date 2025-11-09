using DentalBooking.Domain.Entities;

namespace DentalBooking.Application.Common.Interfaces;

public interface IProcedureRepository
{
    Task<Procedure?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<Procedure>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Procedure procedure, CancellationToken cancellationToken);
    Task DeleteAsync(Procedure procedure, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
