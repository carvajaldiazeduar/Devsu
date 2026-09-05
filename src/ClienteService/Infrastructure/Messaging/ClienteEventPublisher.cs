namespace ClienteService.Infrastructure.Messaging;

using System.Text;
using System.Text.Json;
using ClienteService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public class ClienteEventPublisher : IClienteEventPublisher
{
    private readonly RabbitMqConnection _rabbitMqConnection;
    private readonly ILogger<ClienteEventPublisher> _logger;

    public ClienteEventPublisher(RabbitMqConnection rabbitMqConnection, ILogger<ClienteEventPublisher> logger)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _logger = logger;
    }

    public async Task PublishClienteCreatedAsync(int clienteId, string nombre)
    {
        await PublishAsync("cliente.created", new
        {
            ClienteId = clienteId,
            Nombre = nombre,
            EventDate = DateTime.UtcNow
        });
    }

    public async Task PublishClienteUpdatedAsync(int clienteId, string nombre)
    {
        await PublishAsync("cliente.updated", new
        {
            ClienteId = clienteId,
            Nombre = nombre,
            EventDate = DateTime.UtcNow
        });
    }

    public async Task PublishClienteDeletedAsync(int clienteId)
    {
        await PublishAsync("cliente.deleted", new
        {
            ClienteId = clienteId,
            EventDate = DateTime.UtcNow
        });
    }

    private Task PublishAsync(string routingKey, object message)
    {
        try
        {
            using IModel channel = _rabbitMqConnection.Connection.CreateModel();
            channel.ExchangeDeclare("cliente.exchange", ExchangeType.Topic, durable: true);
            byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: "cliente.exchange", routingKey: routingKey, basicProperties: null, body: body);
            _logger.LogInformation("Evento publicado: {RoutingKey}", routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publicando evento {RoutingKey}, se continúa sin bloquear.", routingKey);
        }

        return Task.CompletedTask;
    }
}
