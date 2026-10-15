using Negocio.Entidades;

namespace Negocio.Interfaces;

public interface IPublicacionRepository
{
    Task<Publicacion> CrearAsync(Publicacion publicacion);
    Task<Publicacion?> ObtenerPorIdAsync(int publicacionId, bool incluirEliminadas = false);
    Task<List<Publicacion>> ObtenerFeedAsync(int? paisId, int? ciudadId);
    Task<List<Publicacion>> ObtenerPorUsuarioAsync(int usuarioId);
    Task<List<Publicacion>> ObtenerTodasAdminAsync(bool incluirEliminadas);
    Task<bool> ExisteCiudadAsync(int ciudadId);

    // --- Favoritos ---
    Task<List<Publicacion>> ObtenerFavoritasAsync(int usuarioId);
    Task<List<int>> ObtenerIdsFavoritasAsync(int usuarioId);
    /// <summary>Guarda la publicación como favorita. Idempotente: si ya estaba guardada no hace nada.</summary>
    Task AgregarFavoritaAsync(int usuarioId, int publicacionId);
    Task QuitarFavoritaAsync(int usuarioId, int publicacionId);
    /// <summary>Baja lógica: marca la publicación como eliminada sin borrar la fila.</summary>
    Task EliminarAsync(int publicacionId);
}
