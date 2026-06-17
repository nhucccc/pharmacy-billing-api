using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Domain.Events;

public sealed class UserCreatedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public string Username { get; }
    public UserRole Role { get; }
    public override string EventType => "user.created";

    public UserCreatedEvent(Guid userId, string username, UserRole role)
    {
        UserId = userId;
        Username = username;
        Role = role;
    }
}
