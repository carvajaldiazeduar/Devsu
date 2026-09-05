namespace CuentaMovimientoService.Domain.Interfaces;

using CuentaMovimientoService.Domain.Entities;

public interface IMovimientoRepository
{
    Task<IEnumerable<Movimiento>> GetMovimientosAsync();
    Task<Movimiento?> GetMovimientoByIdAsync(int id);
    Task<IEnumerable<Movimiento>> GetMovimientosByCuentaAsync(string numeroCuenta);
    Task<Movimiento?> GetUltimoMovimientoAsync(string numeroCuenta);
    Task<IEnumerable<Movimiento>> GetMovimientosEnRangoAsync(DateTime desde, DateTime hasta);
    Task<Movimiento> AddMovimientoAsync(Movimiento movimiento);
    Task UpdateMovimientoAsync(Movimiento movimiento);
}
