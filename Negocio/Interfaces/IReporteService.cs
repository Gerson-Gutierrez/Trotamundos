using Negocio.DTOs;

namespace Negocio.Interfaces;

public interface IReporteService
{
    Task CrearReporteAsync(int usuarioReportadorId, CrearReporteDto dto);
    Task<List<ReporteResponseDto>> ObtenerReportesAdminAsync(string? estado);
    Task ResolverReporteAsync(int reporteId, ResolverReporteDto dto);
    Task<List<NotificacionAdminResponseDto>> ObtenerNotificacionesAdminAsync(bool soloNoLeidas);
    Task MarcarNotificacionLeidaAsync(int notificacionId);
}
