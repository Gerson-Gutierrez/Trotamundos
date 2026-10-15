using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

/// <summary>Gestión de usuarios para el panel de administración.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuariosController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet("admin")]
    public async Task<IActionResult> Listar()
    {
        // Las cuentas de superadministrador se gestionan solo desde su propio panel
        var usuarios = (await _usuarioRepository.ObtenerTodosAsync()).Where(u => !u.EsSuperAdmin);
        return Ok(usuarios.Select(u => new
        {
            u.UsuarioId, u.Nombre, u.Apellido, u.Email, u.Nacionalidad,
            u.FotoPerfilUrl, u.EsAdmin, u.Bloqueado, u.FechaRegistro
        }));
    }

    [HttpPut("admin/{id:int}/bloquear")]
    public Task<IActionResult> Bloquear(int id) => CambiarBloqueo(id, true);

    [HttpPut("admin/{id:int}/desbloquear")]
    public Task<IActionResult> Desbloquear(int id) => CambiarBloqueo(id, false);

    [HttpDelete("admin/{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound(new { mensaje = "El usuario no existe." });
        if (usuario.EsAdmin || usuario.EsSuperAdmin) return BadRequest(new { mensaje = "No se puede eliminar a un administrador." });

        await _usuarioRepository.EliminarAsync(id);
        return Ok(new { mensaje = "Usuario eliminado." });
    }

    private async Task<IActionResult> CambiarBloqueo(int id, bool bloquear)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound(new { mensaje = "El usuario no existe." });
        if ((usuario.EsAdmin || usuario.EsSuperAdmin) && bloquear) return BadRequest(new { mensaje = "No se puede bloquear a un administrador." });

        usuario.Bloqueado = bloquear;
        await _usuarioRepository.GuardarCambiosAsync();
        return Ok(new { mensaje = bloquear ? "Usuario bloqueado." : "Usuario desbloqueado." });
    }
}
