using Datos.Data;
using Microsoft.EntityFrameworkCore;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Datos.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly TrotamundosDbContext _context;

    public ReporteRepository(TrotamundosDbContext context)
    {
        _context = context;
    }

    public async Task CrearReporteAsync(Reporte reporte)
    {
        _context.Reportes.Add(reporte);
        await _context.SaveChangesAsync();
    }

    public async Task<Reporte?> ObtenerPorIdAsync(int reporteId)
    {
        return await _context.Reportes
            .Include(r => r.UsuarioReportador)
            .Include(r => r.Publicacion)
            .FirstOrDefaultAsync(r => r.ReporteId == reporteId);
    }

    public async Task<List<Reporte>> ObtenerTodosPorEstadoAsync(string? estado)
    {
        var query = _context.Reportes
            .Include(r => r.UsuarioReportador)
            .Include(r => r.Publicacion)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(r => r.Estado == estado);

        return await query.OrderByDescending(r => r.FechaReporte).ToListAsync();
    }

    public async Task ActualizarReporteAsync(Reporte reporte)
    {
        // El reporte viene tracked desde ObtenerPorIdAsync: solo se persisten las
        // propiedades modificadas. No usar Update() porque marcaría también la
        // Publicacion incluida y pisaría cambios hechos con ExecuteUpdate.
        if (_context.Entry(reporte).State == EntityState.Detached)
            _context.Reportes.Attach(reporte);

        await _context.SaveChangesAsync();
    }

    public async Task CrearNotificacionAdminAsync(NotificacionAdmin notificacion)
    {
        _context.NotificacionesAdmin.Add(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task<List<NotificacionAdmin>> ObtenerNotificacionesAdminAsync(bool soloNoLeidas)
    {
        var query = _context.NotificacionesAdmin.AsQueryable();
        if (soloNoLeidas) query = query.Where(n => !n.Leida);

        return await query.OrderByDescending(n => n.FechaCreacion).ToListAsync();
    }

    public async Task MarcarNotificacionComoLeidaAsync(int notificacionId)
    {
        var notif = await _context.NotificacionesAdmin.FindAsync(notificacionId);
        if (notif != null)
        {
            notif.Leida = true;
            notif.FechaLectura ??= DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> ContarReportesPorPublicacionAsync(int publicacionId)
    {
        return await _context.Reportes.CountAsync(r => r.PublicacionId == publicacionId);
    }

    public async Task<int> EliminarResueltosAnterioresAAsync(DateTime limiteUtc)
    {
        int reportes = await _context.Reportes
            .Where(r => r.FechaResolucion != null && r.FechaResolucion <= limiteUtc)
            .ExecuteDeleteAsync();

        int alertas = await _context.NotificacionesAdmin
            .Where(n => n.FechaLectura != null && n.FechaLectura <= limiteUtc)
            .ExecuteDeleteAsync();

        return reportes + alertas;
    }
}
