namespace Negocio.Entidades;

/// <summary>Publicación de otro viajero que un usuario guardó en su sección de favoritos.</summary>
public class PublicacionFavorita
{
    public int UsuarioId { get; set; }
    public int PublicacionId { get; set; }
    public DateTime FechaGuardado { get; set; } = DateTime.UtcNow;

    public Usuario Usuario { get; set; } = null!;
    public Publicacion Publicacion { get; set; } = null!;
}
