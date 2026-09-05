using ClienteService.Application.DTOs;

namespace ClienteService.Application.Services;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> GetClientesAsync();
    Task<ClienteDto?> GetClienteByIdAsync(int clienteId);
    Task<ClienteDto> CreateClienteAsync(CrearClienteDto dto);
    Task UpdateClienteAsync(int clienteId, ActualizarClienteDto dto);
    Task DeleteClienteAsync(int clienteId);
}
