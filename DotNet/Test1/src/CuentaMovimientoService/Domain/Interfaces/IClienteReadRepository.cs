namespace CuentaMovimientoService.Domain.Interfaces;

using CuentaMovimientoService.Domain.Entities;

public interface IClienteReadRepository
{
    Task<Cliente?> GetByIdAsync(int clienteId);
    Task UpsertAsync(Cliente cliente);
}
