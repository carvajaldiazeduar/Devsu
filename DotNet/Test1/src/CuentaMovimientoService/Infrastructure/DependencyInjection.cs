namespace CuentaMovimientoService.Infrastructure;

using CuentaMovimientoService.Domain.Interfaces;
using CuentaMovimientoService.Infrastructure.Messaging;
using CuentaMovimientoService.Infrastructure.Persistence;
using CuentaMovimientoService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<CuentasDbConnection>();
        services.AddDbContext<CuentaDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredService<CuentasDbConnection>().DataSource));

        services.AddSingleton<RabbitMqConnection>();
        services.AddScoped<ICuentaRepository, CuentaRepository>();
        services.AddScoped<IMovimientoRepository, MovimientoRepository>();
        services.AddScoped<IClienteReadRepository, ClienteReadRepository>();

        services.AddHostedService<ClienteEventConsumer>();

        return services;
    }
}
