namespace Negocio.Entidades;

public class Publicacion
{
    public int PublicacionId { get; set; }
    public int UsuarioId { get; set; }
    public int CiudadId { get; set; }
    public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

    // Baja lógica: la publicación deja de mostrarse pero se conserva el historial de reportes
    public bool Eliminada { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }

    // Navegación principal
    public Usuario Usuario { get; set; } = null!;
    public Ciudad Ciudad { get; set; } = null!;

    // Relaciones 1 a 1 (Secciones obligatorias de la publicación)
    public Alojamiento? Alojamiento { get; set; }
    public ExperienciaGastronomica? ExperienciaGastronomica { get; set; }
    public Recomendacion? Recomendacion { get; set; }

    // Relación 1 a N (Galería de fotos)
    public ICollection<PublicacionImagen> Imagenes { get; set; } = new List<PublicacionImagen>();

    // Relación con las denuncias que ha recibido esta publicación
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}