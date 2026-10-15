using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

/// <summary>
/// Subida de imágenes (foto de perfil e imágenes de publicaciones).
/// El cliente sube primero la imagen (multipart/form-data), recibe la URL y
/// luego la envía en FotoPerfilUrl / Imagenes[].ImagenUrl.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ArchivosController : ControllerBase
{
    private const long TamanoMaximoBytes = 10 * 1024 * 1024; // 10 MB

    private readonly IFileStorageService _fileStorage;

    public ArchivosController(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    /// <summary>Sube la foto de perfil. Público para poder usarse durante el registro.</summary>
    [HttpPost("perfil")]
    [RequestSizeLimit(TamanoMaximoBytes)]
    public Task<IActionResult> SubirFotoPerfil(IFormFile archivo) => Subir(archivo, "perfiles");

    /// <summary>Sube una imagen de publicación (alojamiento, comida, lugar…). Devuelve la URL a usar en la publicación.</summary>
    [Authorize]
    [HttpPost("publicaciones")]
    [RequestSizeLimit(TamanoMaximoBytes)]
    public Task<IActionResult> SubirImagenPublicacion(IFormFile archivo) => Subir(archivo, "publicaciones");

    /// <summary>Sube varias imágenes de publicación de una vez.</summary>
    [Authorize]
    [HttpPost("publicaciones/multiple")]
    [RequestSizeLimit(TamanoMaximoBytes * 5)]
    public async Task<IActionResult> SubirImagenesPublicacion(List<IFormFile> archivos)
    {
        if (archivos == null || archivos.Count == 0)
            return BadRequest(new { mensaje = "No se recibieron archivos." });

        var urls = new List<string>();
        try
        {
            foreach (var archivo in archivos)
            {
                if (archivo.Length > TamanoMaximoBytes)
                    return BadRequest(new { mensaje = $"El archivo {archivo.FileName} supera los 10 MB." });

                await using var stream = archivo.OpenReadStream();
                urls.Add(await _fileStorage.GuardarArchivoAsync(stream, archivo.FileName, "publicaciones"));
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }

        return Ok(new { urls });
    }

    private async Task<IActionResult> Subir(IFormFile archivo, string carpeta)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { mensaje = "No se recibió ningún archivo." });

        try
        {
            await using var stream = archivo.OpenReadStream();
            var url = await _fileStorage.GuardarArchivoAsync(stream, archivo.FileName, carpeta);
            return Ok(new { url });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
