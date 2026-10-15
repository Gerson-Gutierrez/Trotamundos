using Negocio.DTOs;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Negocio.Services;

public class ReporteService : IReporteService
{
    private const int UmbralDenunciasParaAlerta = 3;
    private static readonly string[] EstadosFinales = { "Resuelto", "Desestimado" };

    private readonly IReporteRepository _reporteRepository;
    private readonly IPublicacionRepository _publicacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ReporteService(
        IReporteRepository reporteRepository,
        IPublicacionRepository publicacionRepository,
        IUsuarioRepository usuarioRepository)
    {
        _reporteRepository = reporteRepository;
        _publicacionRepository = publicacionRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task CrearReporteAsync(int usuarioReportadorId, CrearReporteDto dto)
    {
        var publicacion = await _publicacionRepository.ObtenerPorIdAsync(dto.PublicacionId)
            ?? throw new KeyNotFoundException("La publicación no existe.");

        if (publicacion.UsuarioId == usuarioReportadorId)
            throw new InvalidOperationException("No podés denunciar tu propia publicación.");

        var reporte = new Reporte
        {
            UsuarioReportadorId = usuarioReportadorId,
            PublicacionId = dto.PublicacionId,
            Motivo = dto.Motivo,
            Detalle = dto.Detalle,
            Estado = "Pendiente",
            FechaReporte = DateTime.UtcNow
        };

        await _reporteRepository.CrearReporteAsync(reporte);

        // Generar alerta al admin si la publicación acumula varias denuncias
        int conteoReportes = await _reporteRepository.ContarReportesPorPublicacionAsync(dto.PublicacionId);
        if (conteoReportes >= UmbralDenunciasParaAlerta)
        {
            await _reporteRepository.CrearNotificacionAdminAsync(new NotificacionAdmin
            {
                Titulo = $"Alerta: Publicación #{dto.PublicacionId} acumuló denuncias",
                Mensaje = $"La publicación #{dto.PublicacionId} cuenta actualmente con {conteoReportes} denuncias pendientes de revisión.",
                FechaCreacion = DateTime.UtcNow
            });
        }
    }

    public async Task<List<ReporteResponseDto>> ObtenerReportesAdminAsync(string? estado)
    {
        var reportes = await _reporteRepository.ObtenerTodosPorEstadoAsync(estado);
        return reportes.Select(r => new ReporteResponseDto(
            r.ReporteId,
            r.UsuarioReportadorId,
            $"{r.UsuarioReportador.Nombre} {r.UsuarioReportador.Apellido}",
            r.PublicacionId,
            r.Motivo,
            r.Detalle,
            r.Estado,
            r.AccionTomada,
            r.FechaReporte,
            r.FechaResolucion
        )).ToList();
    }

    public async Task ResolverReporteAsync(int reporteId, ResolverReporteDto dto)
    {
        var reporte = await _reporteRepository.ObtenerPorIdAsync(reporteId)
            ?? throw new KeyNotFoundException("El reporte no existe.");

        Usuario? autorAEliminar = null;
        if (dto.EliminarUsuario)
        {
            autorAEliminar = await _usuarioRepository.ObtenerPorIdAsync(reporte.Publicacion.UsuarioId);
            if (autorAEliminar is { EsAdmin: true } or { EsSuperAdmin: true })
                throw new InvalidOperationException("No se puede eliminar a un administrador.");
        }

        reporte.Estado = dto.Estado;
        reporte.AccionTomada = dto.AccionTomada;
        reporte.FechaResolucion = EstadosFinales.Contains(dto.Estado) ? DateTime.UtcNow : null;

        // Baja lógica: la publicación deja de verse pero el reporte conserva su referencia
        if (dto.EliminarPublicacion)
            await _publicacionRepository.EliminarAsync(reporte.PublicacionId);

        if (dto.BloquearAutor)
        {
            var autor = await _usuarioRepository.ObtenerPorIdAsync(reporte.Publicacion.UsuarioId);
            if (autor != null && !autor.EsAdmin && !autor.EsSuperAdmin)
                autor.Bloqueado = true;
        }

        await _reporteRepository.ActualizarReporteAsync(reporte);

        // Se elimina al final: borra en cascada sus publicaciones y reportes (incluido este)
        if (autorAEliminar != null)
            await _usuarioRepository.EliminarAsync(autorAEliminar.UsuarioId);
    }

    public async Task<List<NotificacionAdminResponseDto>> ObtenerNotificacionesAdminAsync(bool soloNoLeidas)
    {
        var notificaciones = await _reporteRepository.ObtenerNotificacionesAdminAsync(soloNoLeidas);
        return notificaciones.Select(n => new NotificacionAdminResponseDto(
            n.NotificacionId, n.Titulo, n.Mensaje, n.Leida, n.FechaCreacion, n.FechaLectura
        )).ToList();
    }

    public Task MarcarNotificacionLeidaAsync(int notificacionId)
        => _reporteRepository.MarcarNotificacionComoLeidaAsync(notificacionId);
}
