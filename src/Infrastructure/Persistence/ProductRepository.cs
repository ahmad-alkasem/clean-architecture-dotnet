using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = db.Products.AsNoTracking().OrderBy(p => p.Name);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await db.Products.AddAsync(product, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
