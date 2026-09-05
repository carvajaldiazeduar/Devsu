using CuentaMovimientoService.Application.DTOs;
using CuentaMovimientoService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CuentaMovimientoService.API.Controllers;

/// <summary>
/// Controller for managing account movements (movimientos).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoService _movimientoService;

    public MovimientosController(IMovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
    }

    /// <summary>
    /// Retrieves all account movements.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<MovimientoDto> movimientos = await _movimientoService.GetMovimientosAsync();
        return Ok(movimientos);
    }

    /// <summary>
    /// Retrieves the movement history for a specific account number.
    /// </summary>
    /// <param name="numeroCuenta">The account number whose movements should be retrieved.</param>
    /// <returns></returns>
    [HttpGet("cuenta/{numeroCuenta}")]
    public async Task<IActionResult> GetByCuenta(string numeroCuenta)
    {
        IEnumerable<MovimientoDto> movimientos = await _movimientoService.GetMovimientosByCuentaAsync(numeroCuenta);
        return Ok(movimientos);
    }

    /// <summary>
    /// Retrieves a specific account movement by its ID or the movement history for an account number.
    /// </summary>
    /// <param name="id">The movement ID or account number to retrieve.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (int.TryParse(id, out int movimientoId))
        {
            MovimientoDto? movimiento = await _movimientoService.GetMovimientoByIdAsync(movimientoId);
            if (movimiento is not null)
                return Ok(movimiento);
        }

        IEnumerable<MovimientoDto> movimientos = await _movimientoService.GetMovimientosByCuentaAsync(id);
        return Ok(movimientos);
    }

    /// <summary>
    /// Creates a new account movement.
    /// </summary>
    /// <param name="dto">The data transfer object containing the account movement information.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearMovimientoDto dto)
    {
        MovimientoDto created = await _movimientoService.RegistrarMovimientoAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.MovimientoId }, created);
    }

    /// <summary>
    /// Updates an existing account movement by its ID.
    /// </summary>
    /// <param name="id">The ID of the account movement to update.</param>
    /// <param name="dto">The data transfer object containing the updated account movement information.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CrearMovimientoDto dto)
    {
        await _movimientoService.UpdateMovimientoAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing account movement by its ID.
    /// </summary>
    /// <param name="id">The ID of the account movement to partially update.</param>
    /// <param name="dto">The data transfer object containing the updated account movement information.</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> PartialUpdate(int id, [FromBody] CrearMovimientoDto dto)
    {
        await _movimientoService.UpdateMovimientoAsync(id, dto);
        return NoContent();
    }
}
