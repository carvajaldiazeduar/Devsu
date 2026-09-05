using ClienteService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ClienteService.Tests;

public class ClienteTests
{
    [Fact]
    public void Cliente_CreacionCorrecta_DebeTenerValoresIniciales()
    {
        Cliente cliente = new Cliente
        {
            Id = 1,
            Nombre = "Jose Lema",
            Genero = "Masculino",
            Edad = 30,
            Identificacion = "1712345678",
            Direccion = "Otavalo sn y principal",
            Telefono = "098254785",
            Contrasena = "1234"
        };

        cliente.Nombre.Should().Be("Jose Lema");
        cliente.Direccion.Should().Be("Otavalo sn y principal");
        cliente.Estado.Should().BeTrue();
    }

    [Fact]
    public void Cliente_CambiarEstado_DebeActualizarEstado()
    {
        Cliente cliente = new Cliente { Estado = true };

        cliente.CambiarEstado(false);

        cliente.Estado.Should().BeFalse();
    }
}
