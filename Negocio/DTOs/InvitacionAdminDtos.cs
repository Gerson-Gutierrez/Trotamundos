using System.ComponentModel.DataAnnotations;

namespace Negocio.DTOs;

public record CrearInvitacionAdminDto([Required, EmailAddress, MaxLength(200)] string Email, bool EsSuperAdmin = false);

public record AceptarInvitacionAdminDto(
    [Required] string Token,
    [Required, MaxLength(200)] string FullName,
    [Required, MinLength(8)] string Password);

public record InvitacionAdminResponseDto(string Email, DateTime FechaExpiracion, bool EsSuperAdmin);
