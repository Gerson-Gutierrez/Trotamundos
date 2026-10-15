namespace Negocio.Entidades;

public class Restaurante
{
    public int RestauranteId { get; set; }
    public int GastronomiaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? PlaceIdApi { get; set; }

    // Navegación
    public ExperienciaGastronomica ExperienciaGastronomica { get; set; } = null!;
}