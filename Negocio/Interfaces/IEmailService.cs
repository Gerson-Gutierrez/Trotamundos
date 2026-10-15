namespace Negocio.Interfaces;

public interface IEmailService
{
    Task EnviarEmailRecuperacionAsync(string emailDestino, string tokenRecuperacion);
    Task EnviarInvitacionAdminAsync(string emailDestino, string enlace, DateTime expiraUtc, bool esSuperAdmin = false);
}