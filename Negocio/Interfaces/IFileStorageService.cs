namespace Negocio.Interfaces;

public interface IFileStorageService
{
    /// <summary>Guarda el archivo y devuelve la URL relativa pública (ej. /uploads/perfiles/xxx.jpg).</summary>
    Task<string> GuardarArchivoAsync(Stream contenido, string nombreOriginal, string carpetaDestino);
    Task EliminarArchivoAsync(string rutaRelativa);
}
