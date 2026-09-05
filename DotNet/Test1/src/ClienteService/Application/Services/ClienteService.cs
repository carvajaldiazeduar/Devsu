namespace ClienteService.Application.Services;

using global::ClienteService.Application.DTOs;
using global::ClienteService.Domain.Entities;
using global::ClienteService.Domain.Exceptions;

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ClienteService : IClienteService
{
    private readonly Domain.Interfaces.IClienteRepository _repository;
    private readonly Domain.Interfaces.IClienteEventPublisher _eventPublisher;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(
        Domain.Interfaces.IClienteRepository repository,
        Domain.Interfaces.IClienteEventPublisher eventPublisher,
        ILogger<ClienteService> logger)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<IEnumerable<ClienteDto>> GetClientesAsync()
    {
        IEnumerable<Cliente> clientes = await _repository.GetClientesAsync();
        return clientes.Select(ToDto);
    }

    public async Task<ClienteDto?> GetClienteByIdAsync(int clienteId)
    {
        Cliente? cliente = await _repository.GetClienteByIdAsync(clienteId);
        return cliente is null ? null : ToDto(cliente);
    }

    public async Task<ClienteDto> CreateClienteAsync(CrearClienteDto dto)
    {
        Cliente cliente = new()
        {
            Nombre = dto.Nombre,
            Genero = dto.Genero,
            Edad = dto.Edad,
            Identificacion = dto.Identificacion,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            Contrasena = dto.Contrasena,
            Estado = dto.Estado
        };

        Cliente created = await _repository.AddClienteAsync(cliente);
        await _eventPublisher.PublishClienteCreatedAsync(created.Id, created.Nombre);

        _logger.LogInformation("Cliente creado: {ClienteId}", created.Id);
        
        return ToDto(created);
    }

    public async Task UpdateClienteAsync(int clienteId, ActualizarClienteDto dto)
    {
        Cliente cliente = await _repository.GetClienteByIdAsync(clienteId)
            ?? throw new Domain.Exceptions.NotFoundException($"Cliente {clienteId} no encontrado.");

        cliente.Nombre = dto.Nombre;
        cliente.Genero = dto.Genero;
        cliente.Edad = dto.Edad;
        cliente.Identificacion = dto.Identificacion;
        cliente.Direccion = dto.Direccion;
        cliente.Telefono = dto.Telefono;
        cliente.Contrasena = dto.Contrasena;
        cliente.Estado = dto.Estado;

        await _repository.UpdateClienteAsync(cliente);
        await _eventPublisher.PublishClienteUpdatedAsync(clienteId, dto.Nombre);
        
        _logger.LogInformation("Cliente actualizado: {ClienteId}", clienteId);
    }

    public async Task DeleteClienteAsync(int clienteId)
    {
        Cliente cliente = await _repository.GetClienteByIdAsync(clienteId)
            ?? throw new NotFoundException($"Cliente {clienteId} no encontrado.");

        cliente.CambiarEstado(false);

        await _repository.UpdateClienteAsync(cliente);
        await _eventPublisher.PublishClienteDeletedAsync(clienteId);
        
        _logger.LogInformation("Cliente desactivado: {ClienteId}", clienteId);
    }

    private static ClienteDto ToDto(Cliente c)
    {
        return new()
        {
            ClienteId = c.Id,
            Nombre = c.Nombre,
            Genero = c.Genero,
            Edad = c.Edad,
            Identificacion = c.Identificacion,
            Direccion = c.Direccion,
            Telefono = c.Telefono,
            Contrasena = c.Contrasena,
            Estado = c.Estado
        };
    }
}
