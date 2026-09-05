namespace CuentaMovimientoService.Domain.Exceptions;

public class SaldoNoDisponibleException : Exception
{
    public SaldoNoDisponibleException() : base("Saldo no disponible")
    {
    }
}
