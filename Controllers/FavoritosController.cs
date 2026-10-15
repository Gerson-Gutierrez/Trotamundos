using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

/// <summary>Publicaciones de otros viajeros que el usuario guardó en su perfil.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritosController : ControllerBase
{
    private readonly IPublicacionService _publicacionService;

    public FavoritosController(IPublicacionService publicacionService)
    {
        _publicacionService = publicacionService;
    }

    // Listado completo (las guardadas más recientemente primero)
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _publicacionService.ObtenerFavoritasAsync(UsuarioActualId()));
    }

    // Solo los ids: la app los usa para pintar el botón de guardar en cada publicación
    [HttpGet("ids")]
    public async Task<IActionResult> ObtenerIds()
    {
        return Ok(await _publicacionService.ObtenerIdsFavoritasAsync(UsuarioActualId()));
    }

    [HttpPut("{publicacionId:int}")]
    public async Task<IActionResult> Guardar(int publicacionId)
    {
        try
        {
            await _publicacionService.GuardarFavoritaAsync(UsuarioActualId(), publicacionId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpDelete("{publicacionId:int}")]
    public async Task<IActionResult> Quitar(int publicacionId)
    {
        await _publicacionService.QuitarFavoritaAsync(UsuarioActualId(), publicacionId);
        return NoContent();
    }

    private int UsuarioActualId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
