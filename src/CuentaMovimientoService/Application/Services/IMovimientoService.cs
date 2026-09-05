namespace CuentaMovimientoService.Application.Services;

using CuentaMovimientoService.Application.DTOs;

public interface IMovimientoService
{
    Task<IEnumerable<MovimientoDto>> GetMovimientosAsync();
    Task<MovimientoDto?> GetMovimientoByIdAsync(int id);
    Task<IEnumerable<MovimientoDto>> GetMovimientosByCuentaAsync(string numeroCuenta);
    Task<MovimientoDto> RegistrarMovimientoAsync(CrearMovimientoDto dto);
    Task UpdateMovimientoAsync(int id, CrearMovimientoDto dto);
    Task<IEnumerable<ReporteEstadoCuentaDto>> GenerarReporteAsync(DateTime desde, DateTime hasta, int clienteId);
}
