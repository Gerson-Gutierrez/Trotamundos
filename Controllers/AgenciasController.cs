using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Trotamundos.Controllers;

/// <summary>Búsqueda de agencias de turismo, para enlazarlas desde una publicación.</summary>
[ApiController]
[Route("api/[controller]")]
public class AgenciasController : ControllerBase
{
    private readonly IBusquedaAgencias _web;
    private readonly IBusquedaLugares _lugares;

    public AgenciasController(IBusquedaAgencias web, IBusquedaLugares lugares)
    {
        _web = web;
        _lugares = lugares;
    }

    /// <summary>
    /// Busca agencias por nombre uniendo dos fuentes:
    ///  - las agencias de la ciudad registradas en OpenStreetMap (mejor cobertura local),
    ///  - una búsqueda web, que aporta el sitio oficial de las marcas conocidas.
    /// Se hace en el servidor para no exponer claves en la app.
    /// </summary>
    [Authorize]
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar(
        [FromQuery] string q,
        [FromQuery] string? pais = null,
        [FromQuery] string? ciudad = null,
        CancellationToken ct = default)
    {
        var texto = (q ?? string.Empty).Trim();
        if (texto.Length < 2) return Ok(Array.Empty<AgenciaWebDto>());

        // Las dos búsquedas son independientes: si una falla, la otra igual responde
        var porLugar = _lugares.BuscarAsync("agencia", texto, ciudad, pais, ct);
        var porWeb = _web.BuscarAsync(texto, pais, ciudad, ct);
        await Task.WhenAll(porLugar, porWeb);

        var resultado = new List<AgenciaWebDto>();
        var dominios = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var nombres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Primero las agencias locales que tienen sitio cargado: son las más precisas
        foreach (var l in porLugar.Result.Where(l => l.SitioWeb != null))
        {
            if (!Agregar(resultado, dominios, nombres, new AgenciaWebDto(l.Nombre, l.SitioWeb!, l.Direccion, "mapa"))) continue;
        }

        foreach (var a in porWeb.Result)
            Agregar(resultado, dominios, nombres, a);

        // Y por último las locales sin web: sirven para completar el nombre a mano
        foreach (var l in porLugar.Result.Where(l => l.SitioWeb == null))
        {
            if (nombres.Contains(l.Nombre)) continue;
            nombres.Add(l.Nombre);
            resultado.Add(new AgenciaWebDto(l.Nombre, string.Empty, l.Direccion, "sin-web"));
        }

        return Ok(resultado.Take(10));
    }

    private static bool Agregar(
        List<AgenciaWebDto> destino, HashSet<string> dominios, HashSet<string> nombres, AgenciaWebDto agencia)
    {
        if (!Uri.TryCreate(agencia.Url, UriKind.Absolute, out var uri)) return false;
        var dominio = uri.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ? uri.Host[4..] : uri.Host;
        if (!dominios.Add(dominio)) return false;
        nombres.Add(agencia.Nombre);
        destino.Add(agencia);
        return true;
    }
}
