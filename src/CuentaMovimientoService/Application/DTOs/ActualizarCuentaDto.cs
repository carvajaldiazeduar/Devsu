namespace CuentaMovimientoService.Application.DTOs;

public class ActualizarCuentaDto
{
    public string TipoCuenta { get; set; } = string.Empty;
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
}
