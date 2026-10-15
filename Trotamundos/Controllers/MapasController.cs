using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MapasController : ControllerBase
{
    private readonly IMapaService _mapaService;

    public MapasController(IMapaService mapaService)
    {
        _mapaService = mapaService;
    }

    /// <summary>Lista de países. Con ?q= filtra por nombre (buscador alternativo al globo).</summary>
    [HttpGet("paises")]
    public async Task<IActionResult> GetPaises([FromQuery] string? q)
    {
        return Ok(await _mapaService.BuscarPaisesAsync(q));
    }

    /// <summary>
    /// Resuelve el país a partir del código ISO 3166-1 alpha-2 (ej. "AR").
    /// Cesium devuelve este código al tocar un país en el globo (propiedad ISO_A2 del GeoJSON).
    /// </summary>
    [HttpGet("paises/iso/{codigoIso}")]
    public async Task<IActionResult> GetPaisPorIso(string codigoIso)
    {
        var pais = await _mapaService.ObtenerPaisPorCodigoIsoAsync(codigoIso);
        return pais == null ? NotFound(new { mensaje = "País no encontrado." }) : Ok(pais);
    }

    /// <summary>
    /// Ciudades de un país para el desplegable: une las registradas con el catálogo mundial.
    /// ?soloConPublicaciones=true devuelve solo las que ya tienen contenido (filtro del inicio).
    /// ?q= filtra por nombre y ?limite= acota la cantidad (hay países con miles de ciudades).
    /// </summary>
    [HttpGet("paises/{paisId:int}/ciudades")]
    public async Task<IActionResult> GetCiudadesPorPais(
        int paisId,
        [FromQuery] bool soloConPublicaciones = false,
        [FromQuery] string? q = null,
        [FromQuery] int limite = 100)
    {
        return Ok(await _mapaService.ObtenerCiudadesAsync(paisId, soloConPublicaciones, q, Math.Clamp(limite, 1, 500)));
    }

    /// <summary>Registra la ciudad donde está el viajero antes de publicar. Si ya existe, devuelve la existente.</summary>
    [Authorize]
    [HttpPost("ciudades")]
    public async Task<IActionResult> CrearCiudad([FromBody] CiudadCreateDto dto)
    {
        try { return Ok(await _mapaService.ObtenerOCrearCiudadAsync(dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
    }
}
