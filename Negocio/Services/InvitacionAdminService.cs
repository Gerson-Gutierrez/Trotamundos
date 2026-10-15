using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Negocio.DTOs;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Negocio.Services;

public class InvitacionAdminService : IInvitacionAdminService
{
    private static readonly TimeSpan Vigencia = TimeSpan.FromHours(24);
    private const string MensajeTokenInvalido = "El enlace de invitación es inválido, expiró o ya fue utilizado.";

    private readonly IInvitacionAdminRepository _invitaciones;
    private readonly IUsuarioRepository _usuarios;
    private readonly ICorreoBloqueadoRepository _correosBloqueados;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public InvitacionAdminService(
        IInvitacionAdminRepository invitaciones,
        IUsuarioRepository usuarios,
        ICorreoBloqueadoRepository correosBloqueados,
        IEmailService email,
        IConfiguration config)
    {
        _invitaciones = invitaciones;
        _usuarios = usuarios;
        _correosBloqueados = correosBloqueados;
        _email = email;
        _config = config;
    }

    public async Task<InvitacionAdminResponseDto> CrearAsync(int superAdminId, CrearInvitacionAdminDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _usuarios.ObtenerPorEmailAsync(email) != null)
            throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");

        if (await _correosBloqueados.ExisteAsync(email))
            throw new InvalidOperationException("Ese correo electrónico está bloqueado y no puede ser invitado.");

        if (await _invitaciones.ExisteActivaPorEmailAsync(email))
            throw new InvalidOperationException("Ya existe una invitación vigente para ese email.");

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();

        var invitacion = new InvitacionAdmin
        {
            Email = email,
            TokenHash = Hashear(token),
            FechaExpiracion = DateTime.UtcNow.Add(Vigencia),
            CreadaPorUsuarioId = superAdminId,
            EsSuperAdmin = dto.EsSuperAdmin,
            FechaCreacion = DateTime.UtcNow
        };

        await _invitaciones.AgregarAsync(invitacion);

        var baseUrl = _config["Admin:RegistroUrl"] ?? "http://localhost:8081/admin/register";
        var separador = baseUrl.Contains('?') ? '&' : '?';
        try
        {
            await _email.EnviarInvitacionAdminAsync(email, $"{baseUrl}{separador}token={token}", invitacion.FechaExpiracion, invitacion.EsSuperAdmin);
        }
        catch (CorreoNoEnviadoException)
        {
            // Sin correo la invitación no sirve y bloquearía reintentos ("ya existe una invitación vigente")
            await _invitaciones.EliminarAsync(invitacion.InvitacionId);
            throw;
        }

        return new InvitacionAdminResponseDto(email, invitacion.FechaExpiracion, invitacion.EsSuperAdmin);
    }

    public async Task AceptarAsync(AceptarInvitacionAdminDto dto)
    {
        var invitacion = await _invitaciones.ObtenerPorTokenHashAsync(Hashear(dto.Token.Trim()));
        if (invitacion == null || invitacion.Usada || invitacion.FechaExpiracion <= DateTime.UtcNow)
            throw new InvalidOperationException(MensajeTokenInvalido);

        // Puede haberse registrado con ese email entre la invitación y la aceptación
        if (await _usuarios.ObtenerPorEmailAsync(invitacion.Email) != null)
            throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");

        if (await _correosBloqueados.ExisteAsync(invitacion.Email))
            throw new InvalidOperationException("Ese correo electrónico está bloqueado.");

        var partes = dto.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 0)
            throw new InvalidOperationException("El nombre completo es obligatorio.");

        PasswordHasher.Crear(dto.Password, out var hash, out var salt);

        var usuario = new Usuario
        {
            Nombre = partes[0],
            Apellido = partes.Length > 1 ? partes[1] : string.Empty,
            Email = invitacion.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            EsAdmin = !invitacion.EsSuperAdmin,
            EsSuperAdmin = invitacion.EsSuperAdmin
        };

        if (!await _invitaciones.ConsumirYCrearUsuarioAsync(invitacion.InvitacionId, usuario))
            throw new InvalidOperationException(MensajeTokenInvalido);
    }

    private static string Hashear(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
