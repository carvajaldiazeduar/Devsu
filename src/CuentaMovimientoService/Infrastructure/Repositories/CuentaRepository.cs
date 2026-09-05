namespace CuentaMovimientoService.Infrastructure.Repositories;

using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Domain.Interfaces;
using CuentaMovimientoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CuentaRepository : ICuentaRepository
{
    private readonly CuentaDbContext _context;

    public CuentaRepository(CuentaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cuenta>> GetCuentasAsync()
        => await _context.Cuentas.AsNoTracking().ToListAsync();

    public async Task<Cuenta?> GetCuentaByIdAsync(int id)
        => await _context.Cuentas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Cuenta?> GetCuentaByNumeroAsync(string numeroCuenta)
        => await _context.Cuentas.AsNoTracking().FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

    public async Task<IEnumerable<Cuenta>> GetCuentasByClienteAsync(int clienteId)
        => await _context.Cuentas.AsNoTracking().Where(c => c.ClienteId == clienteId).ToListAsync();

    public async Task<Cuenta> AddCuentaAsync(Cuenta cuenta)
    {
        _context.Cuentas.Add(cuenta);
        await _context.SaveChangesAsync();
        return cuenta;
    }

    public async Task UpdateCuentaAsync(Cuenta cuenta)
    {
        _context.Cuentas.Update(cuenta);
        await _context.SaveChangesAsync();
    }
}
