namespace CuentaMovimientoService.Application.DTOs;

public class CrearCuentaDto
{
    public string NumeroCuenta { get; set; } = string.Empty;
    public string TipoCuenta { get; set; } = "Ahorros";
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; } = true;
    public int ClienteId { get; set; }
}
