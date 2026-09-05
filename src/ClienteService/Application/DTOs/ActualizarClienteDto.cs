namespace ClienteService.Application.DTOs;

public class ActualizarClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public bool Estado { get; set; }
}
