namespace CuentaMovimientoService.Infrastructure.Persistence;

using Microsoft.Extensions.Configuration;
using Npgsql;

public sealed class CuentasDbConnection : IDisposable
{
    private NpgsqlDataSource? _dataSource;

    public CuentasDbConnection(IConfiguration configuration)
    {
        string connectionString = BuildConnectionString(configuration);
        _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
    }

    public NpgsqlDataSource DataSource
    {
        get => _dataSource ?? throw new ObjectDisposedException(nameof(CuentasDbConnection));
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        string host = Requerida(configuration, "CuentasDb:Host");
        string port = Requerida(configuration, "CuentasDb:Port");
        string database = Requerida(configuration, "CuentasDb:Database");
        string username = Requerida(configuration, "CuentasDb:Username");
        string password = Requerida(configuration, "CuentasDb:Password");

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