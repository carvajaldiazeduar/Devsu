namespace CuentaMovimientoService.Application.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuentaMovimientoService.Application.DTOs;
using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Domain.Exceptions;
using CuentaMovimientoService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

public class CuentaService : ICuentaService
{
    private readonly ICuentaRepository _cuentaRepository;
    private readonly IClienteReadRepository _clienteReadRepository;
    private readonly ILogger<CuentaService> _logger;

    public CuentaService(
        ICuentaRepository cuentaRepository,
        IClienteReadRepository clienteReadRepository,
        ILogger<CuentaService> logger)
    {
        _cuentaRepository = cuentaRepository;
        _clienteReadRepository = clienteReadRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CuentaDto>> GetCuentasAsync()
    {
        IEnumerable<Cuenta> cuentas = await _cuentaRepository.GetCuentasAsync();
        return cuentas.Select(ToDto);
    }

    public async Task<CuentaDto?> GetCuentaByIdAsync(int id)
    {
        Cuenta? cuenta = await _cuentaRepository.GetCuentaByIdAsync(id);
        return cuenta is null ? null : ToDto(cuenta);
    }

    public async Task<IEnumerable<CuentaDto>> GetCuentasByClienteAsync(int clienteId)
    {
        Cliente? cliente = await _clienteReadRepository.GetByIdAsync(clienteId);
        if (cliente is null)
            throw new NotFoundException($"El cliente {clienteId} no existe.");

        IEnumerable<Cuenta> cuentas = await _cuentaRepository.GetCuentasByClienteAsync(clienteId);
        return cuentas.Select(ToDto);
    }

    public async Task<CuentaDto> CreateCuentaAsync(CrearCuentaDto dto)
    {
        Cliente? cliente = await _clienteReadRepository.GetByIdAsync(dto.ClienteId);
        if (cliente is null)
            throw new NotFoundException($"El cliente {dto.ClienteId} no existe.");

        Cuenta? existente = await _cuentaRepository.GetCuentaByNumeroAsync(dto.NumeroCuenta);
        if (existente is not null)
            throw new CuentaExistenteException($"Ya existe una cuenta con el número {dto.NumeroCuenta}.");

        Cuenta cuenta = new Cuenta
        {
            NumeroCuenta = dto.NumeroCuenta,
            TipoCuenta = dto.TipoCuenta,
            SaldoInicial = dto.SaldoInicial,
            Estado = dto.Estado,
            ClienteId = dto.ClienteId
        };

        Cuenta created = await _cuentaRepository.AddCuentaAsync(cuenta);
        _logger.LogInformation("Cuenta creada: {Numero}", created.NumeroCuenta);
        return ToDto(created);
    }

    public async Task UpdateCuentaAsync(string numeroCuenta, ActualizarCuentaDto dto)
    {
        Cuenta cuenta = await _cuentaRepository.GetCuentaByNumeroAsync(numeroCuenta)
            ?? throw new NotFoundException($"Cuenta {numeroCuenta} no encontrada.");

        cuenta.TipoCuenta = dto.TipoCuenta;
        cuenta.SaldoInicial = dto.SaldoInicial;
        cuenta.Estado = dto.Estado;

        await _cuentaRepository.UpdateCuentaAsync(cuenta);
        _logger.LogInformation("Cuenta actualizada: {Numero}", numeroCuenta);
    }

    private static CuentaDto ToDto(Cuenta c)
    {
        return new CuentaDto
        {
            Id = c.Id,
            NumeroCuenta = c.NumeroCuenta,
            TipoCuenta = c.TipoCuenta,
            SaldoInicial = c.SaldoInicial,
            Estado = c.Estado,
            ClienteId = c.ClienteId
        };
    }
}
