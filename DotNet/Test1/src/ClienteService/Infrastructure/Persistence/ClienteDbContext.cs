namespace ClienteService.Infrastructure.Persistence;

using ClienteService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class ClienteDbContext : DbContext
{
    public ClienteDbContext(DbContextOptions<ClienteDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Persona> Personas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Persona>(entity =>
        {
            entity.ToTable("Personas");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Genero).HasMaxLength(20);
            entity.Property(p => p.Identificacion).HasMaxLength(20).IsRequired();
            entity.Property(p => p.Direccion).HasMaxLength(200);
            entity.Property(p => p.Telefono).HasMaxLength(20);
            entity.HasIndex(p => p.Identificacion).IsUnique();
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.Property(c => c.Contrasena).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Estado).IsRequired();
        });
    }
}