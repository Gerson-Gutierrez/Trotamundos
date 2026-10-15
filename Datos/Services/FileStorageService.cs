using Microsoft.AspNetCore.Hosting;
using Negocio.Interfaces;

namespace Datos.Services;

public class FileStorageService : IFileStorageService
{
    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> GuardarArchivoAsync(Stream contenido, string nombreOriginal, string carpetaDestino)
    {
        if (contenido == null || contenido.Length == 0)
            throw new ArgumentException("El archivo está vacío.");

        string extension = Path.GetExtension(nombreOriginal);
        if (!ExtensionesPermitidas.Contains(extension))
            throw new ArgumentException("Formato no permitido. Solo se aceptan imágenes (jpg, png, webp, gif).");

        // Evita path traversal en la carpeta destino (solo letras/números/guiones)
        string carpeta = new string(carpetaDestino.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
        if (string.IsNullOrEmpty(carpeta))
            throw new ArgumentException("Carpeta de destino inválida.");

        string nombreArchivo = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        string folderPath = Path.Combine(RaizWeb(), "uploads", carpeta);
        Directory.CreateDirectory(folderPath);

        string fullPath = Path.Combine(folderPath, nombreArchivo);
        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await contenido.CopyToAsync(stream);
        }

        return $"/uploads/{carpeta}/{nombreArchivo}";
    }

    public Task EliminarArchivoAsync(string rutaRelativa)
    {
        if (string.IsNullOrEmpty(rutaRelativa)) return Task.CompletedTask;

        string raiz = RaizWeb();
        string fullPath = Path.GetFullPath(Path.Combine(raiz, rutaRelativa.TrimStart('/', '\\')));

        // Solo borra dentro de wwwroot/uploads
        if (fullPath.StartsWith(Path.Combine(raiz, "uploads"), StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private string RaizWeb() =>
        _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}
