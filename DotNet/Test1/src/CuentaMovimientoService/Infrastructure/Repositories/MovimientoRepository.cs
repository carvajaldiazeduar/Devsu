namespace CuentaMovimientoService.Infrastructure.Repositories;

using CuentaMovimientoService.Domain.Entities;
using CuentaMovimientoService.Domain.Interfaces;
using CuentaMovimientoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class MovimientoRepository : IMovimientoRepository
{
    private readonly CuentaDbContext _context;

    public MovimientoRepository(CuentaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movimiento>> GetMovimientosAsync()
        => await _context.Movimientos.AsNoTracking().OrderByDescending(m => m.Fecha).ToListAsync();

    public async Task<Movimiento?> GetMovimientoByIdAsync(int id)
        => await _context.Movimientos.AsNoTracking().FirstOrDefaultAsync(m => m.MovimientoId == id);

    public async Task<IEnumerable<Movimiento>> GetMovimientosByCuentaAsync(string numeroCuenta)
    {
        Cuenta? cuenta = await _context.Cuentas.AsNoTracking().FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
        if (cuenta is null)
            return Enumerable.Empty<Movimiento>();
        return await _context.Movimientos.AsNoTracking().Where(m => m.CuentaId == cuenta.Id).ToListAsync();
    }

    public async Task<Movimiento?> GetUltimoMovimientoAsync(string numeroCuenta)
    {
        Cuenta? cuenta = await _context.Cuentas.AsNoTracking().FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
        if (cuenta is null)
            return null;
        return await _context.Movimientos.AsNoTracking()
            .Where(m => m.CuentaId == cuenta.Id)
            .OrderByDescending(m => m.Fecha)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Movimiento>> GetMovimientosEnRangoAsync(DateTime desde, DateTime hasta)
        => await _context.Movimientos.AsNoTracking()
            .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
            .ToListAsync();

    public async Task<Movimiento> AddMovimientoAsync(Movimiento movimiento)
    {
        _context.Movimientos.Add(movimiento);
        await _context.SaveChangesAsync();
        return movimiento;
    }

    public async Task UpdateMovimientoAsync(Movimiento movimiento)
    {
        _context.Movimientos.Update(movimiento);
        await _context.SaveChangesAsync();
    }
}
