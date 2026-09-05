namespace ClienteService.Infrastructure.Persistence;

using Microsoft.Extensions.Configuration;
using Npgsql;

public sealed class ClientesDbConnection : IDisposable
{
    private NpgsqlDataSource? _dataSource;

    public ClientesDbConnection(IConfiguration configuration)
    {
        string connectionString = BuildConnectionString(configuration);
        _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
    }

    public NpgsqlDataSource DataSource
    {
        get => _dataSource ?? throw new ObjectDisposedException(nameof(ClientesDbConnection));
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        string host = Requerida(configuration, "ClientesDb:Host");
        string port = Requerida(configuration, "ClientesDb:Port");
        string database = Requerida(configuration, "ClientesDb:Database");
        string username = Requerida(configuration, "ClientesDb:Username");
        string password = Requerida(configuration, "ClientesDb:Password");

        return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    }

    private static string Requerida(IConfiguration configuration, string clave)
    {
        return configuration[clave]
            ?? throw new InvalidOperationException($"La configuracion {clave} no esta definida.");
    }

    public void Dispose()
    {
        _dataSource?.Dispose();
        _dataSource = null;
    }
}