namespace Negocio.Entidades;

/// <summary>
/// Agencia de turismo recomendada dentro de una publicación. Lo importante es
/// el nombre y su sitio web, que se muestra como enlace para quien lee la publicación.
/// </summary>
public class AgenciaTurismo
{
    public int AgenciaId { get; set; }
    public int RecomendacionId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? PlaceIdApi { get; set; }
    /// <summary>Página web de la agencia: se muestra como enlace en la publicación.</summary>
    public string? SitioWeb { get; set; }
    /// <summary>Qué excursión o actividad se contrató ahí.</summary>
    public string? Actividad { get; set; }

    // Navegación
    public Recomendacion Recomendacion { get; set; } = null!;
}
