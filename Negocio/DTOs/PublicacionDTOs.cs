 using System.ComponentModel.DataAnnotations;

namespace Negocio.DTOs;

public record AlojamientoCreateDto(
    [Required, MaxLength(200)] string NombreHostelHotel,
    string? DireccionTextual,
    [Range(-90, 90)] decimal Latitud,
    [Range(-180, 180)] decimal Longitud,
    string? PlaceIdApi,
    string? Experiencia);

// Ubicación opcional: el usuario puede nombrar el restaurante sin marcarlo en el mapa
public record RestauranteCreateDto(
    [Required, MaxLength(200)] string Nombre,
    string? Direccion,
    [Range(-90, 90)] decimal? Latitud,
    [Range(-180, 180)] decimal? Longitud,
    string? PlaceIdApi);

public record ExperienciaGastronomicaCreateDto(string? ReseñaGeneral, List<RestauranteCreateDto>? Restaurantes);

// Ubicación opcional, igual que en los restaurantes
public record AgenciaTurismoCreateDto(
    [Required, MaxLength(200)] string Nombre,
    string? Direccion,
    [Range(-90, 90)] decimal? Latitud,
    [Range(-180, 180)] decimal? Longitud,
    string? PlaceIdApi,
    [MaxLength(300)] string? SitioWeb,
    string? Actividad);

public record RecomendacionCreateDto(string? Contenido, List<AgenciaTurismoCreateDto>? Agencias);

public record ImagenCreateDto([Required] string ImagenUrl, string? TipoCategoria);

// Las tres secciones son opcionales de forma individual; una publicación
// debe tener al menos una sección o una imagen con contenido.
public record PublicacionCreateDto(
    [Range(1, int.MaxValue)] int CiudadId,
    AlojamientoCreateDto? Alojamiento,
    ExperienciaGastronomicaCreateDto? ExperienciaGastronomica,
    RecomendacionCreateDto? Recomendacion,
    List<ImagenCreateDto>? Imagenes
);

public record PublicacionResponseDto(
    int PublicacionId,
    int UsuarioId,
    string NombreUsuario,
    string? FotoPerfilUrl,
    string? InstagramUsuario,
    int CiudadId,
    string NombreCiudad,
    int PaisId,
    string NombrePais,
    string? CodigoIsoPais,
    DateTime FechaPublicacion,
    AlojamientoCreateDto? Alojamiento,
    ExperienciaGastronomicaCreateDto? ExperienciaGastronomica,
    RecomendacionCreateDto? Recomendacion,
    List<ImagenCreateDto> Imagenes
);
