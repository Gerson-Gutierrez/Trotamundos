using System.ComponentModel.DataAnnotations;

namespace Negocio.DTOs;

public record CrearReporteDto(
    [Range(1, int.MaxValue)] int PublicacionId,
    [Required, MaxLength(100)] string Motivo,
    string? Detalle
);

public record ReporteResponseDto(
    int ReporteId,
    int UsuarioReportadorId,
    string NombreUsuarioReportador,
    int PublicacionId,
    string Motivo,
    string? Detalle,
    string Estado,
    string? AccionTomada,
    DateTime FechaReporte,
    DateTime? FechaResolucion = null
);

public record ResolverReporteDto(
    [Required] string Estado, // 'EnRevision', 'Resuelto', 'Desestimado'
    string? AccionTomada,
    bool EliminarPublicacion,
    bool BloquearAutor = false,
    bool EliminarUsuario = false
);

public record NotificacionAdminResponseDto(
    int NotificacionId,
    string Titulo,
    string Mensaje,
    bool Leida,
    DateTime FechaCreacion,
    DateTime? FechaLectura = null
);
