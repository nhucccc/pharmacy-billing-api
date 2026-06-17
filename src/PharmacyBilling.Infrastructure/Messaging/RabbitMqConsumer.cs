using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PharmacyBilling.Application.DTOs.Dispensation;
using PharmacyBilling.Application.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PharmacyBilling.Infrastructure.Messaging;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    private const string Exchange = "pharmacy.events";
    private const string Queue = "pharmacy.prescription.created";
    private const string RoutingKey = "prescription.created";

    public RabbitMqConsumer(IServiceProvider serviceProvider, IConfiguration config, ILogger<RabbitMqConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConnectAsync(stoppingToken);
                _logger.LogInformation("RabbitMQ Consumer started. Listening for prescription events...");

                // Keep alive
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ connection failed. Retrying in 10 seconds...");
                await Task.Delay(10_000, stoppingToken);
            }
        }
    }

    private async Task ConnectAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
            UserName = _config["RabbitMQ:Username"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest",
            VirtualHost = _config["RabbitMQ:VirtualHost"] ?? "/"
        };

        _connection = await factory.CreateConnectionAsync(ct);
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);

        await _channel.ExchangeDeclareAsync(Exchange, ExchangeType.Topic, durable: true, cancellationToken: ct);
        await _channel.QueueDeclareAsync(Queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        await _channel.QueueBindAsync(Queue, Exchange, RoutingKey, cancellationToken: ct);
        await _channel.BasicQosAsync(0, 10, false, ct);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleMessageAsync;
        await _channel.BasicConsumeAsync(Queue, false, consumer, ct);
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Body.ToArray());
        _logger.LogInformation("Received prescription.created event: {MessageId}", args.BasicProperties.MessageId);

        try
        {
            var message = JsonSerializer.Deserialize<PrescriptionCreatedMessage>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (message == null)
            {
                _logger.LogWarning("Could not deserialize prescription message");
                await _channel!.BasicNackAsync(args.DeliveryTag, false, false);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var dispensationService = scope.ServiceProvider.GetRequiredService<DispensationService>();

            var request = new CreateDispensationRequest(
                message.PrescriptionId,
                message.PatientId,
                message.DoctorId,
                message.PatientName,
                message.DoctorName,
                message.Diagnosis,
                message.AppointmentId,
                message.Notes,
                message.Items.Select(i => new CreateDispensationItemRequest(
                    i.MedicineId, i.Quantity, i.Dosage, i.Usage, i.DurationDays)).ToList()
            );

            var result = await dispensationService.CreateFromPrescriptionAsync(request);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Dispensation created from prescription {PrescriptionId}", message.PrescriptionId);
                await _channel!.BasicAckAsync(args.DeliveryTag, false);
            }
            else
            {
                _logger.LogWarning("Failed to create dispensation: {Error}", result.Error);
                // Nack with requeue=false (go to dead letter queue if configured)
                await _channel!.BasicNackAsync(args.DeliveryTag, false, false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing prescription message");
            await _channel!.BasicNackAsync(args.DeliveryTag, false, true); // requeue on exception
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel?.IsOpen == true) await _channel.CloseAsync(cancellationToken);
        if (_connection?.IsOpen == true) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
