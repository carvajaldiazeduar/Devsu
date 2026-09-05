using System.Net.Http.Json;
using System.Text.Json;
using CuentaMovimientoService.Application.DTOs;
using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests;

public class CuentaMovimientoIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CuentaMovimientoIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreaCuenta_Y_RegistraMovimiento_FlujoCompleto()
    {
        HttpClient client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CuentaDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddDbContext<CuentaDbContext>(options => options.UseInMemoryDatabase("TestDb"));

                List<ServiceDescriptor> hosted = services.Where(d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService)).ToList();
                foreach (ServiceDescriptor h in hosted)
                    services.Remove(h);
            });
        }).CreateClient();

        using (CuentaDbContext ctx = new CuentaDbContext(new DbContextOptionsBuilder<CuentaDbContext>()
                   .UseInMemoryDatabase("TestDb").Options))
        {
            ctx.Clientes.Add(new Cliente { ClienteId = 1, Nombre = "Marianela Montalvo", Estado = true });
            await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        CrearCuentaDto cuenta = new CrearCuentaDto { NumeroCuenta = "100200", TipoCuenta = "Ahorros", SaldoInicial = 500, Estado = true, ClienteId = 1 };

        HttpResponseMessage crearResp = await client.PostAsJsonAsync("/api/Cuentas", cuenta, cancellationToken: TestContext.Current.CancellationToken);
        crearResp.EnsureSuccessStatusCode();

        CrearMovimientoDto movimiento = new CrearMovimientoDto { NumeroCuenta = "100200", Valor = -100, TipoMovimiento = "Retiro" };
        HttpResponseMessage movResp = await client.PostAsJsonAsync("/api/Movimientos", movimiento, cancellationToken: TestContext.Current.CancellationToken);
        movResp.EnsureSuccessStatusCode();

        string body = await movResp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        MovimientoDto? result = JsonSerializer.Deserialize<MovimientoDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        result.Should().NotBeNull();
        result!.Saldo.Should().Be(400);
    }
}
