namespace CuentaMovimientoService.Application;

using CuentaMovimientoService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICuentaService, Services.CuentaService>();
        services.AddScoped<IMovimientoService, Services.MovimientoService>();
        return services;
    }
}
