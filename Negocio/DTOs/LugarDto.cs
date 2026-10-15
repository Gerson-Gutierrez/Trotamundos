namespace Negocio.DTOs;

/// <summary>
/// Lugar encontrado al buscar alojamientos, restaurantes o agencias.
/// Fuente: "poi" (base de puntos de interés) o "direccion" (geocodificación).
/// </summary>
public record LugarDto(
    string Nombre,
    string? Direccion,
    decimal? Latitud,
    decimal? Longitud,
    string? SitioWeb,
    string? Telefono,
    string Categoria,
    string Etiqueta,
    string Fuente);
