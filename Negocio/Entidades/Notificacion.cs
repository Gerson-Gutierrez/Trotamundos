namespace Negocio.Entidades;

public class NotificacionAdmin
{
    public int NotificacionId { get; set; }
    public string Titulo { get; set; } = string.Empty;   // Ej: 'Alerta: Publicación muy denunciada'
    public string Mensaje { get; set; } = string.Empty;  // Ej: 'La publicación #45 tiene 5 denuncias acumuladas.'
    public bool Leida { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Momento en que se marcó como leída; a las 24 h se elimina automáticamente
    public DateTime? FechaLectura { get; set; }
}