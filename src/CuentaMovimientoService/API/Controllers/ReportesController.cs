using CuentaMovimientoService.Application.DTOs;
using CuentaMovimientoService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CuentaMovimientoService.API.Controllers;

/// <summary>
/// Controller for generating account statement reports (reportes de estado de cuenta).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IMovimientoService _movimientoService;

    public ReportesController(IMovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
    }

    /// <summary>
    /// Generates a report of account statements based on the specified date range and client ID.
    /// </summary>
    /// <param name="fechaInicio"></param>
    /// <param name="fechaFin"></param>
    /// <param name="cliente"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetReporte(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] int cliente)
    {
if (fechaInicio == default || fechaFin == default)
            return BadRequest(new { mensaje = "Debe especificar fechaInicio y fechaFin." });

        if (fechaFin < fechaInicio)
            return BadRequest(new { mensaje = "fechaFin debe ser mayor o igual que fechaInicio." });

        IEnumerable<ReporteEstadoCuentaDto> reporte = await _movimientoService.GenerarReporteAsync(fechaInicio, fechaFin, cliente);
        return Ok(reporte);
    }
}
