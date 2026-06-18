using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> ListAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
