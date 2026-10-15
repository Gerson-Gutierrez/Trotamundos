namespace Negocio.Services;

/// <summary>El correo no pudo enviarse: SMTP sin configurar o el servidor de correo lo rechazó.</summary>
public class CorreoNoEnviadoException : Exception
{
    public CorreoNoEnviadoException(string message, Exception? inner = null) : base(message, inner) { }
}
