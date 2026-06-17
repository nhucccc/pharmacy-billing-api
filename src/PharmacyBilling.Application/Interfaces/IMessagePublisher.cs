namespace PharmacyBilling.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken ct = default) where T : class;
}
