using Application.Common.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Common.Events;

public sealed class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
            await publisher.Publish(Wrap(domainEvent), cancellationToken);
    }

    private static INotification Wrap(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }
}
