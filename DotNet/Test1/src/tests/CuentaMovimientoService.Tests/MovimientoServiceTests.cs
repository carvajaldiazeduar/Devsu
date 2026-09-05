using System;
using System.Collections.Generic;
using System.Linq;
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

public class MovimientoServiceTests
{
    private const string NumeroCuenta1 = "225487";
    private const string NumeroCuenta2 = "478758";

    private readonly Mock<IMovimientoRepository> _movimientoRepository;
    private readonly Mock<ICuentaRepository> _cuentaRepository;
    private readonly Mock<IClienteReadRepository> _clienteReadRepository;
    private readonly MovimientoService _service;

    public MovimientoServiceTests()
    {
        _movimientoRepository = new Mock<IMovimientoRepository>();
        _cuentaRepository = new Mock<ICuentaRepository>();
        _clienteReadRepository = new Mock<IClienteReadRepository>();
        Mock<ILogger<MovimientoService>> logger = new Mock<ILogger<MovimientoService>>();
        _service = new MovimientoService(_movimientoRepository.Object, _cuentaRepository.Object, _clienteReadRepository.Object, logger.Object);
    }

    [Fact]
    public async Task RegistrarMovimiento_ConRetiroMayorAlSaldo_DebeLanzarSaldoNoDisponible()
    {
        Cuenta cuenta = new Cuenta
        {
            Id = 1,
            NumeroCuenta = NumeroCuenta2,
            SaldoInicial = 100
        };
        _cuentaRepository.Setup(r => r.GetCuentaByNumeroAsync(NumeroCuenta2)).ReturnsAsync(cuenta);
        _movimientoRepository.Setup(r => r.GetUltimoMovimientoAsync(NumeroCuenta2)).ReturnsAsync((Movimiento?)null);

        CrearMovimientoDto dto = new CrearMovimientoDto { NumeroCuenta = NumeroCuenta2, Valor = -200, TipoMovimiento = "Retiro" };

        Func<Task> act = async () => await _service.RegistrarMovimientoAsync(dto);

        await act.Should().ThrowAsync<SaldoNoDisponibleException>();
    }

    [Fact]
    public async Task RegistrarMovimiento_ConDeposito_DebeActualizarSaldo()
    {
        Cuenta cuenta = new Cuenta
        {
            Id = 1,
            NumeroCuenta = NumeroCuenta1,
            SaldoInicial = 100
        };
        _cuentaRepository.Setup(r => r.GetCuentaByNumeroAsync(NumeroCuenta1)).ReturnsAsync(cuenta);
        _movimientoRepository.Setup(r => r.GetUltimoMovimientoAsync(NumeroCuenta1)).ReturnsAsync((Movimiento?)null);
        _movimientoRepository.Setup(r => r.AddMovimientoAsync(It.IsAny<Movimiento>()))
            .ReturnsAsync((Movimiento m) => m);

        CrearMovimientoDto dto = new CrearMovimientoDto { NumeroCuenta = NumeroCuenta1, Valor = 600, TipoMovimiento = "Deposito" };

        MovimientoDto result = await _service.RegistrarMovimientoAsync(dto);

result.Saldo.Should().Be(700);
        result.Valor.Should().Be(600);
    }

    [Fact]
    public async Task GenerarReporte_ConFechaHastaDeDia_DebeIncluirElDiaCompleto()
    {
        const int clienteId = 1;
        Cliente cliente = new Cliente { ClienteId = clienteId, Nombre = "Jose Lema" };
        _clienteReadRepository.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);

        Cuenta cuenta = new Cuenta { Id = 1, NumeroCuenta = NumeroCuenta2, SaldoInicial = 2000 };
        _cuentaRepository.Setup(r => r.GetCuentasByClienteAsync(clienteId)).ReturnsAsync(new[] { cuenta });

        DateTime desde = new DateTime(2026, 9, 1);
        DateTime hasta = new DateTime(2026, 9, 4);
        Movimiento movimiento = new Movimiento
        {
            MovimientoId = 1,
            CuentaId = 1,
            Fecha = new DateTime(2026, 9, 4, 22, 12, 43, DateTimeKind.Utc),
            Valor = -575,
            Saldo = 1425
        };

        _movimientoRepository.Setup(r => r.GetMovimientosEnRangoAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((DateTime desdeUtc, DateTime hastaUtc) =>
                movimiento.Fecha >= desdeUtc && movimiento.Fecha <= hastaUtc
                    ? new[] { movimiento }
                    : Array.Empty<Movimiento>());

        IEnumerable<ReporteEstadoCuentaDto> reporte = await _service.GenerarReporteAsync(desde, hasta, clienteId);

        reporte.Should().ContainSingle();
    }
}
