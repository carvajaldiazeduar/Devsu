namespace CuentaMovimientoService.Application.Services;

using CuentaMovimientoService.Application.DTOs;
using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Domain.Exceptions;
using CuentaMovimientoService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

public class MovimientoService : IMovimientoService
{
    private readonly IMovimientoRepository _movimientoRepository;
    private readonly ICuentaRepository _cuentaRepository;
    private readonly IClienteReadRepository _clienteReadRepository;
    private readonly ILogger<MovimientoService> _logger;

    public MovimientoService(
        IMovimientoRepository movimientoRepository,
        ICuentaRepository cuentaRepository,
        IClienteReadRepository clienteReadRepository,
        ILogger<MovimientoService> logger)
    {
        _movimientoRepository = movimientoRepository;
        _cuentaRepository = cuentaRepository;
        _clienteReadRepository = clienteReadRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MovimientoDto>> GetMovimientosAsync()
    {
        IEnumerable<Movimiento> movimientos = await _movimientoRepository.GetMovimientosAsync();
        return movimientos.Select(m => ToDto(m, string.Empty));
    }

    public async Task<MovimientoDto?> GetMovimientoByIdAsync(int id)
    {
        Movimiento? movimiento = await _movimientoRepository.GetMovimientoByIdAsync(id);
        return movimiento is null ? null : ToDto(movimiento, string.Empty);
    }

    public async Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaAsync(string numeroCuenta)
    {
        Cuenta? cuenta = await _cuentaRepository.GetCuentaByNumeroAsync(numeroCuenta);
        if (cuenta is null)
            throw new NotFoundException($"Cuenta {numeroCuenta} no encontrada.");

        IEnumerable<Movimiento> movimientos = await _movimientoRepository.GetMovimientosByCuentaAsync(numeroCuenta);
        return movimientos.Select(m => ToDto(m, numeroCuenta));
    }

    public async Task<MovimientoDto> RegistrarMovimientoAsync(CrearMovimientoDto dto)
    {
        Cuenta cuenta = await _cuentaRepository.GetCuentaByNumeroAsync(dto.NumeroCuenta)
            ?? throw new NotFoundException($"Cuenta {dto.NumeroCuenta} no encontrada.");

        Movimiento? ultimo = await _movimientoRepository.GetUltimoMovimientoAsync(dto.NumeroCuenta);
        decimal saldoActual = ultimo?.Saldo ?? cuenta.SaldoInicial;
        decimal valor = dto.Valor;

        decimal nuevoSaldo = saldoActual + valor;
        if (nuevoSaldo < 0)
            throw new SaldoNoDisponibleException();

        string tipo = valor >= 0 ? "Deposito" : "Retiro";
        if (!string.IsNullOrWhiteSpace(dto.TipoMovimiento))
            tipo = dto.TipoMovimiento;

        Movimiento movimiento = new Movimiento
        {
            Fecha = DateTime.UtcNow,
            TipoMovimiento = tipo,
            Valor = valor,
            Saldo = nuevoSaldo,
            CuentaId = cuenta.Id
        };

        Movimiento created = await _movimientoRepository.AddMovimientoAsync(movimiento);
        _logger.LogInformation("Movimiento registrado para cuenta {Numero}: saldo {Saldo}", dto.NumeroCuenta, nuevoSaldo);
        return ToDto(created, dto.NumeroCuenta);
    }

    public async Task UpdateMovimientoAsync(int id, CrearMovimientoDto dto)
    {
        Movimiento movimiento = await _movimientoRepository.GetMovimientoByIdAsync(id)
            ?? throw new NotFoundException($"Movimiento {id} no encontrado.");

        movimiento.Valor = dto.Valor;
        movimiento.TipoMovimiento = dto.TipoMovimiento;
        await _movimientoRepository.UpdateMovimientoAsync(movimiento);
        _logger.LogInformation("Movimiento actualizado: {Id}", id);
    }

    public async Task<IEnumerable<ReporteEstadoCuentaDto>> GenerarReporteAsync(DateTime desde, DateTime hasta, int clienteId)
    {
        Cliente cliente = await _clienteReadRepository.GetByIdAsync(clienteId)
            ?? throw new NotFoundException($"Cliente {clienteId} no encontrado.");

        DateTime desdeUtc = DateTime.SpecifyKind(desde, DateTimeKind.Utc);
        DateTime hastaUtc = DateTime.SpecifyKind(hasta.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        List<Cuenta> cuentas = (await _cuentaRepository.GetCuentasByClienteAsync(clienteId)).ToList();
        List<ReporteEstadoCuentaDto> reporte = new List<ReporteEstadoCuentaDto>();

        foreach (Cuenta cuenta in cuentas)
        {
            List<Movimiento> movimientos = (await _movimientoRepository.GetMovimientosEnRangoAsync(desdeUtc, hastaUtc))
                .Where(m => m.CuentaId == cuenta.Id)
                .OrderBy(m => m.Fecha)
                .ToList();

            decimal saldoActual = cuenta.SaldoInicial;
            foreach (Movimiento mov in movimientos)
            {
                saldoActual = mov.Saldo;
                reporte.Add(new ReporteEstadoCuentaDto
                {
                    Fecha = mov.Fecha.ToString("dd/MM/yyyy"),
                    Cliente = cliente.Nombre,
                    NumeroCuenta = cuenta.NumeroCuenta,
                    Tipo = cuenta.TipoCuenta,
                    SaldoInicial = cuenta.SaldoInicial,
                    Estado = cuenta.Estado,
                    Movimiento = mov.Valor,
                    SaldoDisponible = saldoActual
                });
            }
        }

        return reporte;
    }

    private static MovimientoDto ToDto(Movimiento m, string numeroCuenta)
    {
        return new MovimientoDto
        {
            MovimientoId = m.MovimientoId,
            Fecha = m.Fecha,
            TipoMovimiento = m.TipoMovimiento,
            Valor = m.Valor,
            Saldo = m.Saldo,
            CuentaId = m.CuentaId,
            NumeroCuenta = numeroCuenta
        };
    }
}
