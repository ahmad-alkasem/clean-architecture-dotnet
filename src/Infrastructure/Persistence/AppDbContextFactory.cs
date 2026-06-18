using Application.Common.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=cleanarchitecture;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options, new NoOpDomainEventDispatcher());
    }

    private sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
