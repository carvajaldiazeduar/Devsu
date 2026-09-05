namespace CuentaMovimientoService.Infrastructure.Persistence;

using CuentaMovimientoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(CuentaDbContext context)
    {
        if (context.Cuentas.Any())
            return;

        await context.Clientes.AddRangeAsync(
            new Cliente { ClienteId = 1, Nombre = "Jose Lema", Estado = true },
            new Cliente { ClienteId = 2, Nombre = "Marianela Montalvo", Estado = true },
            new Cliente { ClienteId = 3, Nombre = "Juan Osorio", Estado = true }
        );
        await context.SaveChangesAsync();

        await context.Cuentas.AddRangeAsync(
            new Cuenta { NumeroCuenta = "478758", TipoCuenta = "Ahorros", SaldoInicial = 2000, Estado = true, ClienteId = 1 },
            new Cuenta { NumeroCuenta = "225487", TipoCuenta = "Corriente", SaldoInicial = 100, Estado = true, ClienteId = 2 },
            new Cuenta { NumeroCuenta = "495878", TipoCuenta = "Ahorros", SaldoInicial = 0, Estado = true, ClienteId = 3 },
            new Cuenta { NumeroCuenta = "496825", TipoCuenta = "Ahorros", SaldoInicial = 540, Estado = true, ClienteId = 2 },
            new Cuenta { NumeroCuenta = "585545", TipoCuenta = "Corriente", SaldoInicial = 1000, Estado = true, ClienteId = 1 }
        );
        await context.SaveChangesAsync();
    }
}