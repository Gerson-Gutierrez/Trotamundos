using Negocio.Entidades;

namespace Negocio.Interfaces;

public interface IInvitacionAdminRepository
{
    Task<bool> ExisteActivaPorEmailAsync(string email);
    Task AgregarAsync(InvitacionAdmin invitacion);
    Task EliminarAsync(int invitacionId);
    Task<InvitacionAdmin?> ObtenerPorTokenHashAsync(string tokenHash);

    /// <summary>
    /// Consume la invitación y crea el usuario en una sola transacción.
    /// Devuelve false si la invitación ya había sido consumida (acceso concurrente).
    /// </summary>
    Task<bool> ConsumirYCrearUsuarioAsync(int invitacionId, Usuario usuario);
}
