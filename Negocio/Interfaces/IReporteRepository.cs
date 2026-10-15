using Negocio.Entidades;

namespace Negocio.Interfaces;

public interface IReporteRepository
{
    Task CrearReporteAsync(Reporte reporte);
    Task<Reporte?> ObtenerPorIdAsync(int reporteId);
    Task<List<Reporte>> ObtenerTodosPorEstadoAsync(string? estado);
    Task ActualizarReporteAsync(Reporte reporte);
    Task CrearNotificacionAdminAsync(NotificacionAdmin notificacion);
    Task<List<NotificacionAdmin>> ObtenerNotificacionesAdminAsync(bool soloNoLeidas);
    Task MarcarNotificacionComoLeidaAsync(int notificacionId);
    Task<int> ContarReportesPorPublicacionAsync(int publicacionId);
    Task<int> EliminarResueltosAnterioresAAsync(DateTime limiteUtc);
}