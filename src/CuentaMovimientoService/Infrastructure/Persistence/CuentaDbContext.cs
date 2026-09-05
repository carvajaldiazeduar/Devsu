namespace CuentaMovimientoService.Infrastructure.Persistence;

using CuentaMovimientoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class CuentaDbContext : DbContext
{
    public CuentaDbContext(DbContextOptions<CuentaDbContext> options) : base(options)
    {
    }

    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Movimiento> Movimientos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("ClientesSync");
            entity.HasKey(c => c.ClienteId);
            entity.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Estado).IsRequired();
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.ToTable("Cuentas");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.NumeroCuenta).HasMaxLength(20).IsRequired();
            entity.HasIndex(c => c.NumeroCuenta).IsUnique();
            entity.Property(c => c.TipoCuenta).HasMaxLength(20).IsRequired();
            entity.Property(c => c.SaldoInicial).HasPrecision(12, 2);
            entity.Property(c => c.Estado).IsRequired();
            entity.Property(c => c.ClienteId).IsRequired();
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.ToTable("Movimientos");
            entity.HasKey(m => m.MovimientoId);
            entity.Property(m => m.Fecha).IsRequired();
            entity.Property(m => m.TipoMovimiento).HasMaxLength(50).IsRequired();
            entity.Property(m => m.Valor).HasPrecision(12, 2).IsRequired();
            entity.Property(m => m.Saldo).HasPrecision(12, 2).IsRequired();

            entity.HasOne(m => m.Cuenta)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.CuentaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
