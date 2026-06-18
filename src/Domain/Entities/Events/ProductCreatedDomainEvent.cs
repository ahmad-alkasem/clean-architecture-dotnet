using Domain.Common;

namespace Domain.Entities.Events;

public sealed record ProductCreatedDomainEvent(Guid ProductId, string Name) : IDomainEvent;
