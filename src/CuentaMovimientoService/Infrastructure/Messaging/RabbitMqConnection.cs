namespace CuentaMovimientoService.Infrastructure.Messaging;

using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

public sealed class RabbitMqConnection : IDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly object _lock = new();
    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqConnection(IConfiguration configuration)
    {
        string hostName = configuration["RabbitMQ:HostName"]
            ?? throw new InvalidOperationException("La configuracion RabbitMQ:HostName no esta definida.");
        string portValue = configuration["RabbitMQ:Port"]
            ?? throw new InvalidOperationException("La configuracion RabbitMQ:Port no esta definida.");
        string userName = configuration["RabbitMQ:UserName"]
            ?? throw new InvalidOperationException("La configuracion RabbitMQ:UserName no esta definida.");
        string password = configuration["RabbitMQ:Password"]
            ?? throw new InvalidOperationException("La configuracion RabbitMQ:Password no esta definida.");

        if (!int.TryParse(portValue, out int port))
            throw new InvalidOperationException("La configuracion RabbitMQ:Port no es un numero valido.");

        _factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };
    }

    public IConnection Connection
    {
        get
        {
            lock (_lock)
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(RabbitMqConnection));

                if (_connection is null || !_connection.IsOpen)
                {
                    _connection?.Dispose();
                    _connection = _factory.CreateConnection();
                }

                return _connection;
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;
            _connection?.Dispose();
            _connection = null;
        }
    }
}