using MediatR;

namespace PharmacyBilling.Domain.Common;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}
