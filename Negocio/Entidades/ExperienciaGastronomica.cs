namespace Negocio.Entidades;

public class ExperienciaGastronomica
{
    public int GastronomiaId { get; set; }
    public int PublicacionId { get; set; }
    public string? ReseñaGeneral { get; set; }

    // Navegación
    public Publicacion Publicacion { get; set; } = null!;
    public ICollection<Restaurante> Restaurantes { get; set; } = new List<Restaurante>();
}