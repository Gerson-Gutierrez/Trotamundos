namespace Negocio.Entidades;

/// <summary>Invitación de un solo uso para crear un usuario con rol Admin.</summary>
public class InvitacionAdmin
{
    public int InvitacionId { get; set; }
    public string Email { get; set; } = string.Empty;

    // Se guarda solo el hash SHA-256 del token: el valor en claro viaja únicamente en el correo.
    public string TokenHash { get; set; } = string.Empty;

    public DateTime FechaExpiracion { get; set; }
    public bool Usada { get; set; } = false;
    // Rol de la cuenta que se crea: false = administrador común, true = superadministrador.
    public bool EsSuperAdmin { get; set; } = false;
    public int CreadaPorUsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Usuario CreadaPor { get; set; } = null!;
}
