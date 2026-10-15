namespace Negocio.Entidades;

public class PublicacionImagen
{
    public int ImagenId { get; set; }
    public int PublicacionId { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public string? TipoCategoria { get; set; } // Ej: 'Alojamiento', 'Comida', 'Lugar'
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

    // Navegación
    public Publicacion Publicacion { get; set; } = null!;
}