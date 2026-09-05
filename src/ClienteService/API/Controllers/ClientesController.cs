using ClienteService.Application.DTOs;
using ClienteService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClienteService.API.Controllers;

/// <summary>
/// Controller for managing clients (clientes).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>
    /// Gets all clients (clientes).
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<ClienteDto> clientes = await _clienteService.GetClientesAsync();
        return Ok(clientes);
    }

    /// <summary>
    /// Gets a client (cliente) by its ID.
    /// </summary>
    /// <param name="id">The ID of the client to retrieve.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        ClienteDto? cliente = await _clienteService.GetClienteByIdAsync(id);
        if (cliente is null)
            return NotFound();
        return Ok(cliente);
    }

    /// <summary>
    /// Creates a new client (cliente).
    /// </summary>
    /// <param name="dto"> The data transfer object containing the information for the new client.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearClienteDto dto)
    {
        ClienteDto created = await _clienteService.CreateClienteAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ClienteId }, created);
    }

    /// <summary>
    /// Updates an existing client (cliente) by its ID.
    /// </summary>
    /// <param name="id">The ID of the client to update.</param>
    /// <param name="dto">The data transfer object containing the updated information.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarClienteDto dto)
    {
        await _clienteService.UpdateClienteAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing client (cliente) by its ID.
    /// </summary>
    /// <param name="id">The ID of the client to partially update.</param>
    /// <param name="dto">The data transfer object containing the updated information.</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> PartialUpdate(int id, [FromBody] ActualizarClienteDto dto)
    {
        await _clienteService.UpdateClienteAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a client (cliente) by its ID.
    /// </summary>
    /// <param name="id">The ID of the client to delete.</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _clienteService.DeleteClienteAsync(id);
        return NoContent();
    }
}
