using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.Interfaces;
using Negocio.Services;

namespace Datos.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <exception cref="CorreoNoEnviadoException">SMTP sin configurar o rechazado por el servidor.</exception>
    public async Task EnviarInvitacionAdminAsync(string emailDestino, string enlace, DateTime expiraUtc, bool esSuperAdmin = false)
    {
        var rol = esSuperAdmin ? "superadministrador" : "administrador";

        var mensaje = CrearMensaje(
            emailDestino,
            $"Invitación como {rol} - Trotamundos",
            $@"
                <p>Hola,</p>
                <p>Te invitaron a ser <b>{rol}</b> de Trotamundos.</p>
                <p><a href=""{WebUtility.HtmlEncode(enlace)}"">Crear mi cuenta de {rol}</a></p>
                <p>Si el enlace no abre, copiá y pegá esta dirección en el navegador:<br>{WebUtility.HtmlEncode(enlace)}</p>
                <p>El enlace es de un solo uso y vence el {expiraUtc:dd/MM/yyyy HH:mm} UTC. Si no esperabas esta invitación, ignorá este correo.</p>");

        await EnviarAsync(mensaje, emailDestino);
    }

    /// <exception cref="CorreoNoEnviadoException">SMTP sin configurar o rechazado por el servidor.</exception>
    public async Task EnviarEmailRecuperacionAsync(string emailDestino, string tokenRecuperacion)
    {
        var mensaje = CrearMensaje(
            emailDestino,
            "Recuperación de contraseña - Trotamundos",
            $@"
                <p>Hola,</p>
                <p>Recibimos una solicitud para restablecer tu contraseña de <b>Trotamundos</b>.</p>
                <p>Ingresá este código en la app para elegir una nueva contraseña:</p>
                <p style=""font-size:20px;letter-spacing:2px""><b>{WebUtility.HtmlEncode(tokenRecuperacion)}</b></p>
                <p>El código vence en 2 horas. Si no pediste este cambio, ignorá este correo.</p>");

        await EnviarAsync(mensaje, emailDestino);
    }

    private MailMessage CrearMensaje(string emailDestino, string asunto, string cuerpoHtml)
    {
        var smtpUser = _config["Email:SmtpUser"];
        var mensaje = new MailMessage
        {
            From = new MailAddress(_config["Email:From"] ?? smtpUser ?? "no-reply@trotamundos.com", "Trotamundos"),
            Subject = asunto,
            Body = cuerpoHtml,
            IsBodyHtml = true
        };
        mensaje.To.Add(emailDestino);
        return mensaje;
    }

    private async Task EnviarAsync(MailMessage mensaje, string emailDestino)
    {
        using var _ = mensaje;

        var smtpHost = _config["Email:SmtpHost"];
        var smtpUser = _config["Email:SmtpUser"];
        var smtpPass = _config["Email:SmtpPass"];
        var smtpPort = int.TryParse(_config["Email:SmtpPort"], out var p) ? p : 587;

        // Antes esto solo se anotaba en el log y la operación parecía exitosa: ahora se informa el fallo
        if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrEmpty(smtpPass))
        {
            _logger.LogError("SMTP sin configurar (Email:SmtpHost / SmtpUser / SmtpPass): no se pudo enviar el correo a {Email}", emailDestino);
            throw new CorreoNoEnviadoException(
                "El servidor no tiene configurado el envío de correos. Configurá Email:SmtpHost, Email:SmtpUser y Email:SmtpPass.");
        }

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true,
            Timeout = 20_000
        };

        try
        {
            await client.SendMailAsync(mensaje);
        }
        catch (Exception ex) when (ex is SmtpException or InvalidOperationException)
        {
            _logger.LogError(ex, "No se pudo enviar el correo a {Email} mediante {Host}:{Puerto}", emailDestino, smtpHost, smtpPort);
            throw new CorreoNoEnviadoException(
                "No se pudo enviar el correo. Revisá la configuración SMTP del servidor (usuario, contraseña y puerto).", ex);
        }
    }
}
