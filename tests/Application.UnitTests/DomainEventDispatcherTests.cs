using Application.Common.Events;
using Domain.Entities.Events;
using MediatR;
using NSubstitute;

namespace Application.UnitTests;

public class DomainEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_publishes_each_event_wrapped_in_a_notification()
    {
        var publisher = Substitute.For<IPublisher>();
        var dispatcher = new DomainEventDispatcher(publisher);
        var domainEvent = new ProductCreatedDomainEvent(Guid.NewGuid(), "Keyboard");

        await dispatcher.DispatchAsync([domainEvent], CancellationToken.None);

        await publisher.Received(1).Publish(
            Arg.Is<INotification>(n =>
                n is DomainEventNotification<ProductCreatedDomainEvent>
                && ((DomainEventNotification<ProductCreatedDomainEvent>)n).DomainEvent == domainEvent),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DispatchAsync_publishes_nothing_for_an_empty_sequence()
    {
        var publisher = Substitute.For<IPublisher>();
        var dispatcher = new DomainEventDispatcher(publisher);

        await dispatcher.DispatchAsync([], CancellationToken.None);

        await publisher.DidNotReceive().Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }
}
