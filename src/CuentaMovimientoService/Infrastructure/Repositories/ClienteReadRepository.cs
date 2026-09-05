namespace CuentaMovimientoService.Infrastructure.Repositories;

using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Domain.Interfaces;
using CuentaMovimientoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ClienteReadRepository : IClienteReadRepository
{
    private readonly CuentaDbContext _context;

    public ClienteReadRepository(CuentaDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(int clienteId)
        => await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.ClienteId == clienteId);

    public async Task UpsertAsync(Cliente cliente)
    {
        Cliente? existing = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteId == cliente.ClienteId);
        if (existing is null)
        {
            _context.Clientes.Add(cliente);
        }
        else
        {
            existing.Nombre = cliente.Nombre;
            existing.Estado = cliente.Estado;
        }
        await _context.SaveChangesAsync();
    }
}
