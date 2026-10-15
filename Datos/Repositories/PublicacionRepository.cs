using Datos.Data;
using Microsoft.EntityFrameworkCore;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Datos.Repositories;

public class PublicacionRepository : IPublicacionRepository
{
    private readonly TrotamundosDbContext _context;

    public PublicacionRepository(TrotamundosDbContext context)
    {
        _context = context;
    }

    public async Task<Publicacion> CrearAsync(Publicacion publicacion)
    {
        await _context.Publicaciones.AddAsync(publicacion);
        await _context.SaveChangesAsync();
        return publicacion;
    }

    public async Task<Publicacion?> ObtenerPorIdAsync(int publicacionId, bool incluirEliminadas = false)
    {
        var query = QueryCompleta();
        if (!incluirEliminadas)
            query = query.Where(p => !p.Eliminada);

        return await query.FirstOrDefaultAsync(p => p.PublicacionId == publicacionId);
    }

    public async Task<List<Publicacion>> ObtenerFeedAsync(int? paisId, int? ciudadId)
    {
        var query = QueryCompleta().Where(p => !p.Eliminada);

        if (ciudadId.HasValue)
            query = query.Where(p => p.CiudadId == ciudadId.Value);
        else if (paisId.HasValue)
            query = query.Where(p => p.Ciudad.PaisId == paisId.Value);

        return await query.OrderByDescending(p => p.FechaPublicacion).ToListAsync();
    }

    public async Task<List<Publicacion>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await QueryCompleta()
            .Where(p => p.UsuarioId == usuarioId && !p.Eliminada)
            .OrderByDescending(p => p.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<List<Publicacion>> ObtenerTodasAdminAsync(bool incluirEliminadas)
    {
        var query = QueryCompleta();
        if (!incluirEliminadas)
            query = query.Where(p => !p.Eliminada);

        return await query.OrderByDescending(p => p.FechaPublicacion).ToListAsync();
    }

    public async Task<bool> ExisteCiudadAsync(int ciudadId)
    {
        return await _context.Ciudades.AnyAsync(c => c.CiudadId == ciudadId);
    }

    public async Task<List<Publicacion>> ObtenerFavoritasAsync(int usuarioId)
    {
        // Las publicaciones eliminadas (baja lógica) dejan de aparecer en favoritos
        return await QueryCompleta()
            .Where(p => !p.Eliminada && _context.PublicacionesFavoritas
                .Any(f => f.UsuarioId == usuarioId && f.PublicacionId == p.PublicacionId))
            .OrderByDescending(p => _context.PublicacionesFavoritas
                .Where(f => f.UsuarioId == usuarioId && f.PublicacionId == p.PublicacionId)
                .Select(f => f.FechaGuardado).FirstOrDefault())
            .ToListAsync();
    }

    public async Task<List<int>> ObtenerIdsFavoritasAsync(int usuarioId)
    {
        return await _context.PublicacionesFavoritas
            .Where(f => f.UsuarioId == usuarioId && !f.Publicacion.Eliminada)
            .Select(f => f.PublicacionId)
            .ToListAsync();
    }

    public async Task AgregarFavoritaAsync(int usuarioId, int publicacionId)
    {
        var yaExiste = await _context.PublicacionesFavoritas
            .AnyAsync(f => f.UsuarioId == usuarioId && f.PublicacionId == publicacionId);
        if (yaExiste) return;

        _context.PublicacionesFavoritas.Add(new PublicacionFavorita
        {
            UsuarioId = usuarioId,
            PublicacionId = publicacionId,
            FechaGuardado = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task QuitarFavoritaAsync(int usuarioId, int publicacionId)
    {
        await _context.PublicacionesFavoritas
            .Where(f => f.UsuarioId == usuarioId && f.PublicacionId == publicacionId)
            .ExecuteDeleteAsync();
    }

    public async Task EliminarAsync(int publicacionId)
    {
        // ExecuteUpdate evita conflictos de tracking cuando la publicación ya fue
        // cargada por otro repositorio dentro del mismo DbContext (p. ej. desde un reporte).
        await _context.Publicaciones
            .Where(p => p.PublicacionId == publicacionId && !p.Eliminada)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Eliminada, true)
                .SetProperty(p => p.FechaEliminacion, DateTime.UtcNow));
    }

    private IQueryable<Publicacion> QueryCompleta()
    {
        return _context.Publicaciones
            .Include(p => p.Usuario)
            .Include(p => p.Ciudad)
                .ThenInclude(c => c.Pais)
            .Include(p => p.Alojamiento)
            .Include(p => p.ExperienciaGastronomica!)
                .ThenInclude(e => e.Restaurantes)
            .Include(p => p.Recomendacion!)
                .ThenInclude(r => r.Agencias)
            .Include(p => p.Imagenes)
            .AsNoTracking();
    }
}
