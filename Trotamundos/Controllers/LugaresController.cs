using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

/// <summary>
/// Búsqueda de alojamientos, restaurantes y agencias por nombre dentro de una ciudad.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LugaresController : ControllerBase
{
    private readonly IBusquedaLugares _busqueda;

    public LugaresController(IBusquedaLugares busqueda)
    {
        _busqueda = busqueda;
    }

    /// <param name="categoria">alojamiento | restaurante | agencia</param>
    [Authorize]
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar(
        [FromQuery] string categoria,
        [FromQuery] string q,
        [FromQuery] string? ciudad = null,
        [FromQuery] string? pais = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
            return Ok(Array.Empty<object>());

        return Ok(await _busqueda.BuscarAsync(categoria, q, ciudad, pais, ct));
    }

    /// <summary>
    /// Prepara el listado de la ciudad para que la primera búsqueda no espere.
    /// La app lo llama al entrar al paso correspondiente del asistente.
    /// </summary>
    [Authorize]
    [HttpPost("precargar")]
    public async Task<IActionResult> Precargar(
        [FromQuery] string categoria,
        [FromQuery] string? ciudad = null,
        [FromQuery] string? pais = null,
        CancellationToken ct = default)
    {
        var cantidad = await _busqueda.PrecargarAsync(categoria, ciudad, pais, ct);
        return Ok(new { cantidad });
    }
}
