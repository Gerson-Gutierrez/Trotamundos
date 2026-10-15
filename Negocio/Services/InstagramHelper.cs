using System.Text.RegularExpressions;

namespace Negocio.Services;

/// <summary>Normaliza lo que el usuario escribe como Instagram a un nombre de usuario válido.</summary>
public static partial class InstagramHelper
{
    // Reglas de Instagram: 1-30 caracteres entre letras, números, punto y guion bajo
    [GeneratedRegex(@"^[A-Za-z0-9._]{1,30}$")]
    private static partial Regex UsuarioValido();

    /// <summary>
    /// Acepta "usuario", "@usuario" o "https://instagram.com/usuario/". Devuelve null si viene vacío
    /// y lanza ArgumentException si no es un usuario de Instagram válido.
    /// </summary>
    public static string? Normalizar(string? entrada)
    {
        if (string.IsNullOrWhiteSpace(entrada)) return null;

        var texto = entrada.Trim();

        // Enlace al perfil: nos quedamos con el primer tramo de la ruta
        var enlace = Regex.Match(texto, @"^(?:https?://)?(?:www\.)?instagram\.com/([^/?#\s]+)", RegexOptions.IgnoreCase);
        if (enlace.Success) texto = enlace.Groups[1].Value;

        texto = texto.TrimStart('@');

        if (!UsuarioValido().IsMatch(texto))
            throw new ArgumentException("El usuario de Instagram no es válido. Usá solo letras, números, puntos y guiones bajos (máx. 30).");

        return texto;
    }
}
