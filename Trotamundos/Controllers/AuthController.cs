using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        try { return Ok(await _authService.RegistrarAsync(dto)); }
        catch (InvalidOperationException ex) { return Conflict(new { mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try { return Ok(await _authService.LoginAsync(dto)); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { mensaje = ex.Message }); }
    }

    [HttpPost("solicitar-recuperacion")]
    public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitudRecuperacionDto dto)
    {
        // Siempre responde 200 para no revelar qué correos están registrados
        await _authService.SolicitarRecuperacionPasswordAsync(dto.Email);
        return Ok(new { mensaje = "Si el correo está registrado, se envió un token de recuperación." });
    }

    [HttpPost("resetear-password")]
    public async Task<IActionResult> ResetearPassword([FromBody] CambiarPasswordDto dto)
    {
        var exito = await _authService.ResetearPasswordAsync(dto);
        if (!exito) return BadRequest(new { mensaje = "Token inválido o expirado." });
        return Ok(new { mensaje = "Contraseña actualizada con éxito." });
    }

    [Authorize]
    [HttpGet("perfil")]
    public async Task<IActionResult> ObtenerPerfil()
    {
        var perfil = await _authService.ObtenerPerfilAsync(UsuarioActualId());
        return perfil == null ? NotFound() : Ok(perfil);
    }

    [Authorize]
    [HttpPut("perfil")]
    public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto dto)
    {
        try { return Ok(await _authService.ActualizarPerfilAsync(UsuarioActualId(), dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [Authorize]
    [HttpDelete("cuenta")]
    public async Task<IActionResult> EliminarCuenta([FromBody] EliminarCuentaDto dto)
    {
        try
        {
            await _authService.EliminarCuentaAsync(UsuarioActualId(), dto.Password);
            return Ok(new { mensaje = "Tu cuenta fue eliminada." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    private int UsuarioActualId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
