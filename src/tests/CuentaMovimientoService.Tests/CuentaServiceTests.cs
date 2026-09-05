using System.Threading.Tasks;
using CuentaMovimientoService.Application.DTOs;
using CuentaMovimientoService.Application.Services;
using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Domain.Exceptions;
using CuentaMovimientoService.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CuentaMovimientoService.Tests;

public class CuentaServiceTests
{
    private readonly Mock<ICuentaRepository> _cuentaRepository;
    private readonly Mock<IClienteReadRepository> _clienteReadRepository;
    private readonly CuentaService _service;

    public CuentaServiceTests()
    {
        _cuentaRepository = new Mock<ICuentaRepository>();
        _clienteReadRepository = new Mock<IClienteReadRepository>();
        Mock<ILogger<CuentaService>> logger = new Mock<ILogger<CuentaService>>();
        _service = new CuentaService(_cuentaRepository.Object, _clienteReadRepository.Object, logger.Object);
    }

    [Fact]
    public async Task CreateCuentaAsync_ConNumeroDuplicado_DebeLanzarCuentaExistente()
    {
        const string numeroCuenta = "478758";
        Cliente cliente = new Cliente { ClienteId = 1, Nombre = "Jose Lema" };
        _clienteReadRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
        _cuentaRepository.Setup(r => r.GetCuentaByNumeroAsync(numeroCuenta))
            .ReturnsAsync(new Cuenta { Id = 1, NumeroCuenta = numeroCuenta });

        CrearCuentaDto dto = new CrearCuentaDto { ClienteId = 1, NumeroCuenta = numeroCuenta };

        Func<Task> act = async () => await _service.CreateCuentaAsync(dto);

        await act.Should().ThrowAsync<CuentaExistenteException>()
            .WithMessage("Ya existe una cuenta con el número 478758.");
    }
}