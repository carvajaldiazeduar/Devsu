namespace CuentaMovimientoService.Infrastructure.Messaging;

using System.Text;
using System.Text.Json;
using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class ClienteEventConsumer : BackgroundService
{
    private readonly RabbitMqConnection _rabbitMqConnection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ClienteEventConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public ClienteEventConsumer(
        RabbitMqConnection rabbitMqConnection,
        IServiceScopeFactory scopeFactory,
        ILogger<ClienteEventConsumer> logger)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consumidor de eventos de cliente iniciado, esperando conexión a RabbitMQ.");

        int reintentos = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = _rabbitMqConnection.Connection;
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare("cliente.exchange", ExchangeType.Topic, durable: true);
                _channel.QueueDeclare("cliente.sync.queue", durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind("cliente.sync.queue", "cliente.exchange", "cliente.*");

                AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        byte[] body = ea.Body.ToArray();
                        string json = Encoding.UTF8.GetString(body);
                        ClienteInstantanea? message = JsonSerializer.Deserialize<ClienteInstantanea>(json);
                        if (message is null)
                            return;

                        using IServiceScope scope = _scopeFactory.CreateScope();
                        IClienteReadRepository repo = scope.ServiceProvider.GetRequiredService<IClienteReadRepository>();

                        await repo.UpsertAsync(new Cliente
                        {
                            ClienteId = message.ClienteId,
                            Nombre = message.Nombre,
                            Estado = true
                        });

                        _logger.LogInformation("Cliente sincronizado: {ClienteId}", message.ClienteId);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error procesando evento de cliente.");
                        _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                    }
                };

                _channel.BasicConsume(queue: "cliente.sync.queue", autoAck: false, consumer: consumer);
                _logger.LogInformation("Consumidor de eventos de cliente listo.");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo conectar a RabbitMQ, se reintentará en 5 segundos. Intento {Intento}.", reintentos + 1);
                reintentos++;
                _channel?.Close();
                _connection?.Close();
                _channel = null;
                _connection = null;
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }

    private sealed class ClienteInstantanea
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
    }
}
