using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher dispatcher)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = entities.SelectMany(entity => entity.DomainEvents).ToList();

        var affected = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in entities)
            entity.ClearDomainEvents();

        await dispatcher.DispatchAsync(domainEvents, cancellationToken);

        return affected;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired().HasMaxLength(200);
            b.Property(p => p.Price).HasPrecision(18, 2);
            b.Ignore(p => p.DomainEvents);
            b.HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            b.Ignore(c => c.DomainEvents);
        });

        base.OnModelCreating(modelBuilder);
    }
}
