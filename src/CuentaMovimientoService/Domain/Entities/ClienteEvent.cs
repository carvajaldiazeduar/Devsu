namespace CuentaMovimientoService.Domain.Entities;

public class ClienteEvent
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? TipoEvento { get; set; }
    public DateTime EventDate { get; set; }
}
