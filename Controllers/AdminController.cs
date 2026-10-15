using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

/// <summary>
/// Activación de cuentas administrativas a partir de una invitación de un solo uso.
/// Las invitaciones las envía el superadministrador (ver SuperAdminController).
/// </summary>
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IInvitacionAdminService _invitaciones;

    public AdminController(IInvitacionAdminService invitaciones)
    {
        _invitaciones = invitaciones;
    }

    [AllowAnonymous]
    [HttpPost("accept-invitation")]
    public async Task<IActionResult> AceptarInvitacion([FromBody] AceptarInvitacionAdminDto dto)
    {
        try
        {
            await _invitaciones.AceptarAsync(dto);
            return Ok(new { mensaje = "Cuenta creada. Ya podés iniciar sesión." });
        }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }
}
