namespace CuentaMovimientoService.Domain.Exceptions;

public class CuentaExistenteException : Exception
{
    public CuentaExistenteException(string message) : base(message)
    {
    }
}