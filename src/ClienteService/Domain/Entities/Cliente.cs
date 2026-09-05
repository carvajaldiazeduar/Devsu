namespace ClienteService.Domain.Entities;

public class Cliente : Persona
{
    public string Contrasena { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;

    public void CambiarEstado(bool estado)
    {
        Estado = estado;
    }
}