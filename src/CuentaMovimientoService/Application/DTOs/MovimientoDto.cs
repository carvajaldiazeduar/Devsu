namespace CuentaMovimientoService.Application.DTOs;

public class MovimientoDto
{
    public int MovimientoId { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Saldo { get; set; }
    public int CuentaId { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
}
