namespace CuentaMovimientoService.Application.DTOs;

public class ReporteEstadoCuentaDto
{
    public string Fecha { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public decimal Movimiento { get; set; }
    public decimal SaldoDisponible { get; set; }
}
