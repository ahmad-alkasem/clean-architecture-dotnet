using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
