using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> ListAsync(CancellationToken ct = default) =>
        await db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        db.Categories.AnyAsync(c => c.Id == id, ct);

    public async Task AddAsync(Category category, CancellationToken ct = default) =>
        await db.Categories.AddAsync(category, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
