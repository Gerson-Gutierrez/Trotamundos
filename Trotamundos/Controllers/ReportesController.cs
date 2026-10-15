using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    // Endpoint para que cualquier viajero autenticado pueda denunciar una publicación
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CrearReporte([FromBody] CrearReporteDto dto)
    {
        try
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _reporteService.CrearReporteAsync(usuarioId, dto);
            return Ok(new { mensaje = "Reporte registrado. El contenido será revisado por el equipo de moderación." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    // --- ENDPOINTS ADMINISTRATIVOS (Solo Rol Admin) ---

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> ObtenerReportes([FromQuery] string? estado)
    {
        var reportes = await _reporteService.ObtenerReportesAdminAsync(estado);
        return Ok(reportes);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/{id:int}/resolver")]
    public async Task<IActionResult> ResolverReporte(int id, [FromBody] ResolverReporteDto dto)
    {
        try
        {
            await _reporteService.ResolverReporteAsync(id, dto);
            return Ok(new { mensaje = "Estado del reporte actualizado correctamente." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/notificaciones")]
    public async Task<IActionResult> ObtenerNotificacionesAdmin([FromQuery] bool soloNoLeidas = false)
    {
        var notificaciones = await _reporteService.ObtenerNotificacionesAdminAsync(soloNoLeidas);
        return Ok(notificaciones);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/notificaciones/{id:int}/leida")]
    public async Task<IActionResult> MarcarNotificacionLeida(int id)
    {
        await _reporteService.MarcarNotificacionLeidaAsync(id);
        return NoContent();
    }
}
