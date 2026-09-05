namespace ClienteService.Domain.Interfaces;

using ClienteService.Domain.Entities;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetClientesAsync();
    Task<Cliente?> GetClienteByIdAsync(int clienteId);
    Task<Cliente> AddClienteAsync(Cliente cliente);
    Task UpdateClienteAsync(Cliente cliente);
    Task DeleteClienteAsync(int clienteId);
}
