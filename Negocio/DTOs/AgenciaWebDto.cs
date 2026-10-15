namespace Negocio.DTOs;

/// <summary>
/// Resultado de buscar una agencia de turismo en la web.
/// Fuente indica de dónde salió: "web" (buscador), "mapa" (OpenStreetMap) o "marca".
/// </summary>
public record AgenciaWebDto(string Nombre, string Url, string? Descripcion, string Fuente);
