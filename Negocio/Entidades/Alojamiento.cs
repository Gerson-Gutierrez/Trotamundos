namespace Negocio.Entidades;

public class Alojamiento
{
    public int AlojamientoId { get; set; }
    public int PublicacionId { get; set; }
    public string NombreHostelHotel { get; set; } = string.Empty;
    public string? DireccionTextual { get; set; }
    public decimal Latitud { get; set; } 
    public decimal Longitud { get; set; }
    public string? PlaceIdApi { get; set; }
    public string? Experiencia { get; set; }

    // Navegación
    public Publicacion Publicacion { get; set; } = null!;
}