using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PharmacyBilling.Application.Interfaces;
using RabbitMQ.Client;

namespace PharmacyBilling.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly IConfiguration _config;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private bool _disposed;

    public RabbitMqPublisher(IConfiguration config, ILogger<RabbitMqPublisher> logger)
    {
        _config = config;
        _logger = logger;
    }

    private async Task EnsureConnectedAsync()
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true) return;

        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
            UserName = _config["RabbitMQ:Username"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest",
            VirtualHost = _config["RabbitMQ:VirtualHost"] ?? "/"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken ct = default) where T : class
    {
        try
        {
            await EnsureConnectedAsync();

            await _channel!.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);

            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel!.BasicPublishAsync(exchange, routingKey, false, props, body, ct);
            _logger.LogInformation("Published message to {Exchange}/{RoutingKey}", exchange, routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to {Exchange}/{RoutingKey}", exchange, routingKey);
            // Don't rethrow - publishing failures should not break business flow
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _channel?.Dispose();
        _connection?.Dispose();
        _disposed = true;
    }
}
