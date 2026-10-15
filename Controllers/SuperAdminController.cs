using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.DTOs;
using Negocio.Interfaces;
using Negocio.Services;

namespace Trotamundos.Controllers;

/// <summary>
/// Panel del superadministrador: altas de administradores y superadministradores,
/// recuperación de contraseñas, bajas con bloqueo de correo e invitaciones.
/// Es un rol aparte de "Admin": los administradores comunes no acceden a estos endpoints.
/// </summary>
[ApiController]
[Route("api/superadmin")]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : ControllerBase
{
    private readonly ISuperAdminService _superAdmin;
    private readonly IInvitacionAdminService _invitaciones;

    public SuperAdminController(ISuperAdminService superAdmin, IInvitacionAdminService invitaciones)
    {
        _superAdmin = superAdmin;
        _invitaciones = invitaciones;
    }

    [HttpGet("cuentas")]
    public async Task<IActionResult> ListarCuentas() => Ok(await _superAdmin.ListarCuentasAsync());

    [HttpPost("invitaciones")]
    public async Task<IActionResult> Invitar([FromBody] CrearInvitacionAdminDto dto)
    {
        try
        {
            var respuesta = await _invitaciones.CrearAsync(UsuarioActualId(), dto);
            var rol = respuesta.EsSuperAdmin ? "superadministrador" : "administrador";
            return Ok(new { mensaje = $"Invitación como {rol} enviada a {respuesta.Email}.", respuesta.FechaExpiracion });
        }
        catch (InvalidOperationException ex) { return Conflict(new { mensaje = ex.Message }); }
        catch (CorreoNoEnviadoException ex) { return StatusCode(StatusCodes.Status502BadGateway, new { mensaje = ex.Message }); }
    }

    [HttpPost("cuentas/{id:int}/recuperar-password")]
    public async Task<IActionResult> RecuperarPassword(int id)
    {
        try
        {
            await _superAdmin.RecuperarPasswordAsync(id);
            return Ok(new { mensaje = "Se envió un código de recuperación al correo de la cuenta." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (CorreoNoEnviadoException ex) { return StatusCode(StatusCodes.Status502BadGateway, new { mensaje = ex.Message }); }
    }

    [HttpDelete("administradores/{id:int}")]
    public async Task<IActionResult> EliminarAdministrador(int id, [FromBody] EliminarAdministradorDto? dto)
    {
        try
        {
            await _superAdmin.EliminarAdministradorAsync(UsuarioActualId(), id, dto?.Motivo);
            return Ok(new { mensaje = "Administrador eliminado y su correo fue bloqueado." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpGet("correos-bloqueados")]
    public async Task<IActionResult> ListarCorreosBloqueados() => Ok(await _superAdmin.ListarCorreosBloqueadosAsync());

    [HttpDelete("correos-bloqueados")]
    public async Task<IActionResult> DesbloquearCorreo([FromQuery] string email)
    {
        var quitado = await _superAdmin.DesbloquearCorreoAsync(email);
        return quitado
            ? Ok(new { mensaje = "Correo desbloqueado." })
            : NotFound(new { mensaje = "El correo no estaba bloqueado." });
    }

    private int UsuarioActualId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
