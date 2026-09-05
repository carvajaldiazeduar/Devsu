namespace ClienteService.Infrastructure.Repositories;

using ClienteService.Domain.Entities;
using ClienteService.Domain.Interfaces;
using ClienteService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ClienteRepository : IClienteRepository
{
    private readonly ClienteDbContext _context;

    public ClienteRepository(ClienteDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> GetClientesAsync()
    {
        return await _context.Clientes.AsNoTracking().ToListAsync();
    }

    public async Task<Cliente?> GetClienteByIdAsync(int clienteId)
    {
        return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clienteId);
    }

    public async Task<Cliente> AddClienteAsync(Cliente cliente)
    {
        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

        Cliente clienteEntity = new Cliente
        {
            Nombre = cliente.Nombre,
            Genero = cliente.Genero,
            Edad = cliente.Edad,
            Identificacion = cliente.Identificacion,
            Direccion = cliente.Direccion,
            Telefono = cliente.Telefono,
            Contrasena = cliente.Contrasena,
            Estado = cliente.Estado
        };
        _context.Clientes.Add(clienteEntity);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
        return clienteEntity;
    }

    public async Task UpdateClienteAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClienteAsync(int clienteId)
    {
        Persona? persona = await _context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == clienteId);
        if (persona is not null)
        {
            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
        }
    }
}
