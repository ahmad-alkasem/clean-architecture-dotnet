using Application.Common.Events;
using Domain.Entities.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Events;

public sealed partial class ProductCreatedDomainEventLogger
    : INotificationHandler<DomainEventNotification<ProductCreatedDomainEvent>>
{
    private readonly ILogger<ProductCreatedDomainEventLogger> _logger;

    public ProductCreatedDomainEventLogger(ILogger<ProductCreatedDomainEventLogger> logger) => _logger = logger;

    public Task Handle(DomainEventNotification<ProductCreatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        LogProductCreated(domainEvent.ProductId, domainEvent.Name);
        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product created: {ProductId} ({Name})")]
    private partial void LogProductCreated(Guid productId, string name);
}
