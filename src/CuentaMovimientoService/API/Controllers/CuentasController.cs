using CuentaMovimientoService.Application.DTOs;
using CuentaMovimientoService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CuentaMovimientoService.API.Controllers;

/// <summary>
/// Controller for managing accounts (cuentas).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly ICuentaService _cuentaService;

    public CuentasController(ICuentaService cuentaService)
    {
        _cuentaService = cuentaService;
    }

    /// <summary>
    /// Retrieves all accounts (cuentas).
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<CuentaDto> cuentas = await _cuentaService.GetCuentasAsync();
        return Ok(cuentas);
    }

    /// <summary>
    /// Retrieves an account (cuenta) by its ID.
    /// </summary>
    /// <param name="id">The ID of the account to retrieve.</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        CuentaDto? cuenta = await _cuentaService.GetCuentaByIdAsync(id);
        if (cuenta is null)
            return NotFound();
        return Ok(cuenta);
    }

    /// <summary>
    /// Retrieves accounts (cuentas) associated with a specific client (cliente).
    /// </summary>
    /// <param name="clienteId">The ID of the client for which to retrieve accounts.</param>
    /// <returns></returns>
[HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetByCliente(int clienteId)
    {
        IEnumerable<CuentaDto> cuentas = await _cuentaService.GetCuentasByClienteAsync(clienteId);
        return Ok(cuentas);
    }

    /// <summary>
    /// Creates a new account (cuenta).
    /// </summary>
    /// <param name="dto">The data transfer object containing the account information.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCuentaDto dto)
    {
        CuentaDto created = await _cuentaService.CreateCuentaAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing account (cuenta) identified by its account number (numeroCuenta).
    /// </summary>
    /// <param name="numeroCuenta">The account number of the account to update.</param>
    /// <param name="dto">The data transfer object containing the updated account information.</param>
    /// <returns></returns>
    [HttpPut("{numeroCuenta}")]
    public async Task<IActionResult> Update(string numeroCuenta, [FromBody] ActualizarCuentaDto dto)
    {
        await _cuentaService.UpdateCuentaAsync(numeroCuenta, dto);
        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing account (cuenta) identified by its account number (numeroCuenta).
    /// </summary>
    /// <param name="numeroCuenta">The account number of the account to partially update.</param>
    /// <param name="dto">The data transfer object containing the updated account information.</param>
    /// <returns></returns>
    [HttpPatch("{numeroCuenta}")]
    public async Task<IActionResult> PartialUpdate(string numeroCuenta, [FromBody] ActualizarCuentaDto dto)
    {
        await _cuentaService.UpdateCuentaAsync(numeroCuenta, dto);
        return NoContent();
    }
}
