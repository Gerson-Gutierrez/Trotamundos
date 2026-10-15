using Negocio.Entidades;

namespace Negocio.Interfaces;

public interface IMapaRepository
{
    Task<List<Pais>> ObtenerPaisesAsync(string? filtroNombre);
    Task<Pais?> ObtenerPaisPorIdAsync(int paisId);
    Task<Pais?> ObtenerPaisPorCodigoIsoAsync(string codigoIso);
    Task<List<Ciudad>> ObtenerCiudadesPorPaisAsync(int paisId, bool soloConPublicaciones);
    Task<Ciudad?> ObtenerCiudadPorNombreAsync(int paisId, string nombre);
    Task<Ciudad> CrearCiudadAsync(Ciudad ciudad);
    Task ActualizarCiudadAsync(Ciudad ciudad);
}
