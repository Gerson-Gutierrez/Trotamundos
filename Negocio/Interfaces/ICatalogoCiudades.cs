namespace Negocio.Interfaces;

/// <summary>
/// Catálogo mundial de ciudades por código ISO de país. Permite ofrecer las
/// ciudades de cualquier país aunque todavía nadie haya publicado ahí.
/// </summary>
public interface ICatalogoCiudades
{
    /// <summary>Nombres de ciudades del país, ordenados alfabéticamente. Vacío si no hay datos.</summary>
    IReadOnlyList<string> ObtenerPorIso(string? codigoIso);
}
