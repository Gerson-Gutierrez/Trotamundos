namespace Negocio.Entidades;

public class Ciudad
{
    public int CiudadId { get; set; }
    public int PaisId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? PlaceIdApi { get; set; }

    // Navegación
    public Pais Pais { get; set; } = null!;
    public ICollection<Publicacion> Publicaciones { get; set; } = new List<Publicacion>();
}