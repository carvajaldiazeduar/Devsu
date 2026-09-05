namespace CuentaMovimientoService.Application.Services;

using CuentaMovimientoService.Application.DTOs;

public interface ICuentaService
{
    Task<IEnumerable<CuentaDto>> GetCuentasAsync();
    Task<CuentaDto?> GetCuentaByIdAsync(int id);
    Task<IEnumerable<CuentaDto>> GetCuentasByClienteAsync(int clienteId);
    Task<CuentaDto> CreateCuentaAsync(CrearCuentaDto dto);
    Task UpdateCuentaAsync(string numeroCuenta, ActualizarCuentaDto dto);
}
