namespace CuentaMovimientoService.Domain.Entities;

public class Cliente
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
}
