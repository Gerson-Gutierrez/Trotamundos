using System.ComponentModel.DataAnnotations;

namespace Negocio.DTOs;

public record CuentaAdministrativaDto(
    int UsuarioId, string Nombre, string Apellido, string Email,
    bool EsSuperAdmin, bool Bloqueado, DateTime FechaRegistro);

public record EliminarAdministradorDto([MaxLength(300)] string? Motivo);

public record CorreoBloqueadoDto(string Email, string? Motivo, DateTime FechaBloqueo);
