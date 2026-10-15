namespace Negocio.Entidades;

public class Reporte
{
    public int ReporteId { get; set; }

    // Usuario que realiza la denuncia
    public int UsuarioReportadorId { get; set; }

    // Publicación que está siendo denunciada
    public int PublicacionId { get; set; }

    public string Motivo { get; set; } = string.Empty; // Ej: 'Spam', 'Contenido Ofensivo', 'Ubicación Falsa'
    public string? Detalle { get; set; }              // Explicación textual del problema

    // Estado del reporte para el panel de administración
    public string Estado { get; set; } = "Pendiente"; // 'Pendiente', 'EnRevision', 'Resuelto', 'Desestimado'
    public string? AccionTomada { get; set; }         // Ej: 'Publicación Eliminada', 'Sin Acción', 'Usuario Bloqueado'

    public DateTime FechaReporte { get; set; } = DateTime.UtcNow;

    // Momento en que se resolvió/desestimó; a las 24 h se elimina automáticamente
    public DateTime? FechaResolucion { get; set; }

    // Propiedades de Navegación
    public Usuario UsuarioReportador { get; set; } = null!;
    public Publicacion Publicacion { get; set; } = null!;
}