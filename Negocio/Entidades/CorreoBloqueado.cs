namespace Negocio.Entidades;

/// <summary>
/// Correo vetado: no puede volver a registrarse ni ser invitado como administrador.
/// Se crea cuando un superadministrador elimina a un administrador común.
/// </summary>
public class CorreoBloqueado
{
    public int CorreoBloqueadoId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Motivo { get; set; }
    public int BloqueadoPorUsuarioId { get; set; }
    public DateTime FechaBloqueo { get; set; } = DateTime.UtcNow;
}
