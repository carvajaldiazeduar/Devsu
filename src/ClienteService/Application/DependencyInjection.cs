namespace ClienteService.Application;

using ClienteService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        IServiceCollection serviceCollection = services.AddScoped<IClienteService, ClienteService>();
        return services;
    }
}
