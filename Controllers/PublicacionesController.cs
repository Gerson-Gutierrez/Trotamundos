using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicacionesController : ControllerBase
{
    private readonly IPublicacionService _publicacionService;

    public PublicacionesController(IPublicacionService publicacionService)
    {
        _publicacionService = publicacionService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] PublicacionCreateDto dto)
    {
        try
        {
            var res = await _publicacionService.CrearPublicacionAsync(UsuarioActualId(), dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = res.PublicacionId }, res);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // Feed público del inicio: filtra por país (al tocar el globo) o por ciudad (desplegable)
    [HttpGet]
    public async Task<IActionResult> ObtenerFeed([FromQuery] int? paisId, [FromQuery] int? ciudadId)
    {
        var feed = await _publicacionService.ObtenerFeedPorFiltroAsync(paisId, ciudadId);
        return Ok(feed);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var publicacion = await _publicacionService.ObtenerPorIdAsync(id);
        return publicacion == null ? NotFound() : Ok(publicacion);
    }

    [Authorize]
    [HttpGet("mias")]
    public async Task<IActionResult> ObtenerMias()
    {
        return Ok(await _publicacionService.ObtenerMisPublicacionesAsync(UsuarioActualId()));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _publicacionService.EliminarAsync(id, UsuarioActualId(), User.IsInRole("Admin"));
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = ex.Message }); }
    }

    // --- ADMIN ---

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> ObtenerTodasAdmin([FromQuery] bool incluirEliminadas = false)
    {
        return Ok(await _publicacionService.ObtenerTodasAdminAsync(incluirEliminadas));
    }

    private int UsuarioActualId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
