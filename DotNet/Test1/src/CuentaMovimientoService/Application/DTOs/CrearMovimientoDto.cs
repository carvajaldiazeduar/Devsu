namespace CuentaMovimientoService.Application.DTOs;

public class CrearMovimientoDto
{
    public string NumeroCuenta { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
}
