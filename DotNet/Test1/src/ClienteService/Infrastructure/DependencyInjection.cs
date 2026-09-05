namespace ClienteService.Infrastructure;

using ClienteService.Domain.Interfaces;
using ClienteService.Infrastructure.Messaging;
using ClienteService.Infrastructure.Persistence;
using ClienteService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ClientesDbConnection>();
        services.AddDbContext<ClienteDbContext>((sp, options) =>
            options.UseNpgsql(sp.GetRequiredService<ClientesDbConnection>().DataSource));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddSingleton<RabbitMqConnection>();
        services.AddScoped<IClienteEventPublisher, ClienteEventPublisher>();

        return services;
    }
}
