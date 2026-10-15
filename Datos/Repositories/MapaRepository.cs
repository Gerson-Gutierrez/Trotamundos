using Datos.Data;
using Microsoft.EntityFrameworkCore;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Datos.Repositories;

public class MapaRepository : IMapaRepository
{
    private readonly TrotamundosDbContext _context;

    public MapaRepository(TrotamundosDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pais>> ObtenerPaisesAsync(string? filtroNombre)
    {
        var query = _context.Paises.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtroNombre))
        {
            var patron = $"%{filtroNombre.Trim()}%";
            query = query.Where(p => EF.Functions.ILike(p.Nombre, patron));
        }

        return await query.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Pais?> ObtenerPaisPorIdAsync(int paisId)
    {
        return await _context.Paises.AsNoTracking().FirstOrDefaultAsync(p => p.PaisId == paisId);
    }

    public async Task<Pais?> ObtenerPaisPorCodigoIsoAsync(string codigoIso)
    {
        var iso = codigoIso.Trim().ToUpper();
        return await _context.Paises.AsNoTracking().FirstOrDefaultAsync(p => p.CodigoIso == iso);
    }

    public async Task<List<Ciudad>> ObtenerCiudadesPorPaisAsync(int paisId, bool soloConPublicaciones)
    {
        var query = _context.Ciudades.AsNoTracking().Where(c => c.PaisId == paisId);

        // Para el desplegable del inicio solo interesan las ciudades que tienen contenido
        if (soloConPublicaciones)
            query = query.Where(c => c.Publicaciones.Any(p => !p.Eliminada));

        return await query.OrderBy(c => c.Nombre).ToListAsync();
    }

    public async Task<Ciudad?> ObtenerCiudadPorNombreAsync(int paisId, string nombre)
    {
        var nombreNormalizado = nombre.Trim();
        return await _context.Ciudades
            .FirstOrDefaultAsync(c => c.PaisId == paisId && EF.Functions.ILike(c.Nombre, nombreNormalizado));
    }

    public async Task<Ciudad> CrearCiudadAsync(Ciudad ciudad)
    {
        _context.Ciudades.Add(ciudad);
        await _context.SaveChangesAsync();
        return ciudad;
    }

    public async Task ActualizarCiudadAsync(Ciudad ciudad)
    {
        // La ciudad viene tracked desde ObtenerCiudadPorNombreAsync
        await _context.SaveChangesAsync();
    }
}
