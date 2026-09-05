namespace CuentaMovimientoService.Domain.Interfaces;

using CuentaMovimientoService.Domain.Entities;

public interface ICuentaRepository
{
    Task<IEnumerable<Cuenta>> GetCuentasAsync();
    Task<Cuenta?> GetCuentaByIdAsync(int id);
    Task<Cuenta?> GetCuentaByNumeroAsync(string numeroCuenta);
    Task<IEnumerable<Cuenta>> GetCuentasByClienteAsync(int clienteId);
    Task<Cuenta> AddCuentaAsync(Cuenta cuenta);
    Task UpdateCuentaAsync(Cuenta cuenta);
}
