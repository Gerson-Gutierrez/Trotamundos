namespace Negocio.Entidades;

public class Recomendacion
{
    public int RecomendacionId { get; set; }
    public int PublicacionId { get; set; }
    public string Contenido { get; set; } = string.Empty;

    // Navegación
    public Publicacion Publicacion { get; set; } = null!;
    public ICollection<AgenciaTurismo> Agencias { get; set; } = new List<AgenciaTurismo>();
}
