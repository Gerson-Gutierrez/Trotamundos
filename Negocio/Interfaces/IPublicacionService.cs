using Negocio.DTOs;

namespace Negocio.Interfaces;

public interface IPublicacionService
{
    Task<PublicacionResponseDto> CrearPublicacionAsync(int usuarioId, PublicacionCreateDto dto);
    Task<PublicacionResponseDto?> ObtenerPorIdAsync(int publicacionId);
    Task<List<PublicacionResponseDto>> ObtenerFeedPorFiltroAsync(int? paisId, int? ciudadId);
    Task<List<PublicacionResponseDto>> ObtenerMisPublicacionesAsync(int usuarioId);
    Task<List<PublicacionResponseDto>> ObtenerTodasAdminAsync(bool incluirEliminadas);

    // --- Favoritos ---
    Task<List<PublicacionResponseDto>> ObtenerFavoritasAsync(int usuarioId);
    Task<List<int>> ObtenerIdsFavoritasAsync(int usuarioId);
    /// <summary>Guarda una publicación de otra persona en favoritos.</summary>
    Task GuardarFavoritaAsync(int usuarioId, int publicacionId);
    Task QuitarFavoritaAsync(int usuarioId, int publicacionId);
    /// <summary>Elimina (baja lógica) una publicación. Solo el autor o un admin pueden hacerlo.</summary>
    Task EliminarAsync(int publicacionId, int usuarioSolicitanteId, bool esAdmin);
}
