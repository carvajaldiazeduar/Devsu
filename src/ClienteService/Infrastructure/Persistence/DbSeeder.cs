namespace ClienteService.Infrastructure.Persistence;

using ClienteService.Domain.Entities;

public static class DbSeeder
{
    public static async Task SeedAsync(ClienteDbContext context)
    {
        if (context.Clientes.Any())
            return;

        await context.Clientes.AddRangeAsync(
            new Cliente { Nombre = "Jose Lema", Genero = "Masculino", Edad = 35, Identificacion = "1700000001", Direccion = "Otavalo sn y principal", Telefono = "098254785", Contrasena = "1234", Estado = true },
            new Cliente { Nombre = "Marianela Montalvo", Genero = "Femenino", Edad = 32, Identificacion = "1700000002", Direccion = "Amazonas y NNUU", Telefono = "097548965", Contrasena = "5678", Estado = true },
            new Cliente { Nombre = "Juan Osorio", Genero = "Masculino", Edad = 40, Identificacion = "1700000003", Direccion = "13 Junio y Equinoccial", Telefono = "098874587", Contrasena = "1245", Estado = true }
        );
        await context.SaveChangesAsync();
    }
}