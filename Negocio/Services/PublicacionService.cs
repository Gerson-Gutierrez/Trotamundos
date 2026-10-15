using Negocio.DTOs;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Negocio.Services;

public class PublicacionService : IPublicacionService
{
    private readonly IPublicacionRepository _publicacionRepository;

    public PublicacionService(IPublicacionRepository publicacionRepository)
    {
        _publicacionRepository = publicacionRepository;
    }

    public async Task<PublicacionResponseDto> CrearPublicacionAsync(int usuarioId, PublicacionCreateDto dto)
    {
        if (!await _publicacionRepository.ExisteCiudadAsync(dto.CiudadId))
            throw new ArgumentException("La ciudad indicada no existe. Registrala primero desde /api/mapas/ciudades.");

        bool tieneContenido = dto.Alojamiento != null
            || dto.ExperienciaGastronomica != null
            || dto.Recomendacion != null
            || (dto.Imagenes?.Count ?? 0) > 0;

        if (!tieneContenido)
            throw new ArgumentException("La publicación debe tener al menos una sección o una imagen.");

        var publicacion = new Publicacion
        {
            UsuarioId = usuarioId,
            CiudadId = dto.CiudadId,
            FechaPublicacion = DateTime.UtcNow,
            Alojamiento = dto.Alojamiento != null ? new Alojamiento
            {
                NombreHostelHotel = dto.Alojamiento.NombreHostelHotel,
                DireccionTextual = dto.Alojamiento.DireccionTextual,
                Latitud = dto.Alojamiento.Latitud,
                Longitud = dto.Alojamiento.Longitud,
                PlaceIdApi = dto.Alojamiento.PlaceIdApi,
                Experiencia = dto.Alojamiento.Experiencia
            } : null,
            ExperienciaGastronomica = dto.ExperienciaGastronomica != null ? new ExperienciaGastronomica
            {
                ReseñaGeneral = dto.ExperienciaGastronomica.ReseñaGeneral,
                Restaurantes = (dto.ExperienciaGastronomica.Restaurantes ?? new()).Select(r => new Restaurante
                {
                    Nombre = r.Nombre,
                    Direccion = r.Direccion,
                    Latitud = r.Latitud,
                    Longitud = r.Longitud,
                    PlaceIdApi = r.PlaceIdApi
                }).ToList()
            } : null,
            Recomendacion = dto.Recomendacion != null ? new Recomendacion
            {
                Contenido = dto.Recomendacion.Contenido ?? string.Empty,
                Agencias = (dto.Recomendacion.Agencias ?? new()).Select(a => new AgenciaTurismo
                {
                    Nombre = a.Nombre,
                    Direccion = a.Direccion,
                    Latitud = a.Latitud,
                    Longitud = a.Longitud,
                    PlaceIdApi = a.PlaceIdApi,
                    SitioWeb = a.SitioWeb,
                    Actividad = a.Actividad
                }).ToList()
            } : null,
            Imagenes = (dto.Imagenes ?? new()).Select(img => new PublicacionImagen
            {
                ImagenUrl = img.ImagenUrl,
                TipoCategoria = img.TipoCategoria,
                FechaSubida = DateTime.UtcNow
            }).ToList()
        };

        var creada = await _publicacionRepository.CrearAsync(publicacion);
        var completa = await _publicacionRepository.ObtenerPorIdAsync(creada.PublicacionId);

        return MapearAResponseDto(completa!);
    }

    public async Task<PublicacionResponseDto?> ObtenerPorIdAsync(int publicacionId)
    {
        var p = await _publicacionRepository.ObtenerPorIdAsync(publicacionId);
        return p == null ? null : MapearAResponseDto(p);
    }

    public async Task<List<PublicacionResponseDto>> ObtenerFeedPorFiltroAsync(int? paisId, int? ciudadId)
    {
        var resultados = await _publicacionRepository.ObtenerFeedAsync(paisId, ciudadId);
        return resultados.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<PublicacionResponseDto>> ObtenerMisPublicacionesAsync(int usuarioId)
    {
        var resultados = await _publicacionRepository.ObtenerPorUsuarioAsync(usuarioId);
        return resultados.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<PublicacionResponseDto>> ObtenerTodasAdminAsync(bool incluirEliminadas)
    {
        var resultados = await _publicacionRepository.ObtenerTodasAdminAsync(incluirEliminadas);
        return resultados.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<PublicacionResponseDto>> ObtenerFavoritasAsync(int usuarioId)
    {
        var resultados = await _publicacionRepository.ObtenerFavoritasAsync(usuarioId);
        return resultados.Select(MapearAResponseDto).ToList();
    }

    public Task<List<int>> ObtenerIdsFavoritasAsync(int usuarioId) =>
        _publicacionRepository.ObtenerIdsFavoritasAsync(usuarioId);

    public async Task GuardarFavoritaAsync(int usuarioId, int publicacionId)
    {
        var publicacion = await _publicacionRepository.ObtenerPorIdAsync(publicacionId)
            ?? throw new KeyNotFoundException("La publicación no existe.");

        if (publicacion.UsuarioId == usuarioId)
            throw new InvalidOperationException("No podés guardar tus propias publicaciones en favoritos.");

        await _publicacionRepository.AgregarFavoritaAsync(usuarioId, publicacionId);
    }

    public Task QuitarFavoritaAsync(int usuarioId, int publicacionId) =>
        _publicacionRepository.QuitarFavoritaAsync(usuarioId, publicacionId);

    public async Task EliminarAsync(int publicacionId, int usuarioSolicitanteId, bool esAdmin)
    {
        var publicacion = await _publicacionRepository.ObtenerPorIdAsync(publicacionId)
            ?? throw new KeyNotFoundException("La publicación no existe.");

        if (!esAdmin && publicacion.UsuarioId != usuarioSolicitanteId)
            throw new UnauthorizedAccessException("No podés eliminar una publicación de otro usuario.");

        await _publicacionRepository.EliminarAsync(publicacionId);
    }

    private static PublicacionResponseDto MapearAResponseDto(Publicacion p)
    {
        return new PublicacionResponseDto(
            p.PublicacionId,
            p.UsuarioId,
            $"{p.Usuario.Nombre} {p.Usuario.Apellido}",
            p.Usuario.FotoPerfilUrl,
            p.Usuario.InstagramUsuario,
            p.CiudadId,
            p.Ciudad.Nombre,
            p.Ciudad.PaisId,
            p.Ciudad.Pais.Nombre,
            p.Ciudad.Pais.CodigoIso,
            p.FechaPublicacion,
            p.Alojamiento != null ? new AlojamientoCreateDto(
                p.Alojamiento.NombreHostelHotel, p.Alojamiento.DireccionTextual,
                p.Alojamiento.Latitud, p.Alojamiento.Longitud,
                p.Alojamiento.PlaceIdApi, p.Alojamiento.Experiencia
            ) : null,
            p.ExperienciaGastronomica != null ? new ExperienciaGastronomicaCreateDto(
                p.ExperienciaGastronomica.ReseñaGeneral,
                p.ExperienciaGastronomica.Restaurantes.Select(r => new RestauranteCreateDto(
                    r.Nombre, r.Direccion, r.Latitud, r.Longitud, r.PlaceIdApi
                )).ToList()
            ) : null,
            p.Recomendacion != null ? new RecomendacionCreateDto(
                p.Recomendacion.Contenido,
                p.Recomendacion.Agencias.Select(a => new AgenciaTurismoCreateDto(
                    a.Nombre, a.Direccion, a.Latitud, a.Longitud, a.PlaceIdApi, a.SitioWeb, a.Actividad
                )).ToList()
            ) : null,
            p.Imagenes.Select(i => new ImagenCreateDto(i.ImagenUrl, i.TipoCategoria)).ToList()
        );
    }
}
