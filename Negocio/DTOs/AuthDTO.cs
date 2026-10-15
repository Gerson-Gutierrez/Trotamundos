using System.ComponentModel.DataAnnotations;

namespace Negocio.DTOs;

public record RegistroDto(
    [Required, MaxLength(100)] string Nombre,
    [Required, MaxLength(100)] string Apellido,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required, MaxLength(100)] string Nacionalidad,
    string? FotoPerfilUrl,
    // Opcional: usuario de Instagram (con o sin @, o el enlace al perfil)
    [MaxLength(200)] string? Instagram = null);

public record LoginDto([Required, EmailAddress] string Email, [Required] string Password);

public record AuthResponseDto(int UsuarioId, string Nombre, string Apellido, string Email, string Nacionalidad, string? FotoPerfilUrl, bool EsAdmin, bool EsSuperAdmin, string Token, string? InstagramUsuario = null);

public record SolicitudRecuperacionDto([Required, EmailAddress] string Email);

public record CambiarPasswordDto([Required] string Token, [Required, MinLength(8)] string NuevaPassword);

public record UsuarioPerfilDto(int UsuarioId, string Nombre, string Apellido, string Email, string Nacionalidad, string? FotoPerfilUrl, bool EsAdmin, bool EsSuperAdmin, DateTime FechaRegistro, string? InstagramUsuario = null);

public record EliminarCuentaDto([Required] string Password);

public record ActualizarPerfilDto(
    [Required, MaxLength(100)] string Nombre,
    [Required, MaxLength(100)] string Apellido,
    [Required, MaxLength(100)] string Nacionalidad,
    string? FotoPerfilUrl,
    // null o vacío quita la red social
    [MaxLength(200)] string? Instagram = null);
