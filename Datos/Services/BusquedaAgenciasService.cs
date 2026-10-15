using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Datos.Services;

/// <summary>
/// Busca agencias de turismo en la web. Usa el primer proveedor disponible:
///   1. Brave Search      (BusquedaWeb:Brave:ApiKey)
///   2. Google Custom Search (BusquedaWeb:Google:ApiKey + :Cx)
///   3. DuckDuckGo        (sin clave, pero solo reconoce marcas conocidas)
/// y suma las agencias mapeadas en OpenStreetMap que tengan sitio web cargado,
/// que son las que mejor cubren los negocios chicos de cada ciudad.
///
/// Sin clave configurada la búsqueda funciona igual, con menos alcance.
/// </summary>
public class BusquedaAgenciasService : IBusquedaAgencias
{
    private const int MaximoResultados = 8;

    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<BusquedaAgenciasService> _logger;

    public BusquedaAgenciasService(HttpClient http, IConfiguration config, ILogger<BusquedaAgenciasService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task<List<AgenciaWebDto>> BuscarAsync(
        string consulta, string? codigoPais, string? ciudad, CancellationToken ct = default)
    {
        var texto = (consulta ?? string.Empty).Trim();
        if (texto.Length < 3) return new List<AgenciaWebDto>();

        var resultados = new List<AgenciaWebDto>();

        // Cada fuente se consulta por separado: si una falla, las demás siguen sirviendo
        foreach (var fuente in FuentesDisponibles(texto, codigoPais, ciudad))
        {
            try { resultados.AddRange(await fuente(ct)); }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { _logger.LogWarning(ex, "Falló una fuente de búsqueda de agencias"); }

            if (resultados.Count >= MaximoResultados) break;
        }

        return Unificar(resultados);
    }

    private IEnumerable<Func<CancellationToken, Task<List<AgenciaWebDto>>>> FuentesDisponibles(
        string texto, string? codigoPais, string? ciudad)
    {
        var consultaWeb = ArmarConsulta(texto, ciudad);

        var claveBrave = _config["BusquedaWeb:Brave:ApiKey"];
        if (!string.IsNullOrWhiteSpace(claveBrave))
            yield return ct => BuscarEnBrave(consultaWeb, claveBrave, codigoPais, ct);

        var claveGoogle = _config["BusquedaWeb:Google:ApiKey"];
        var cx = _config["BusquedaWeb:Google:Cx"];
        if (!string.IsNullOrWhiteSpace(claveGoogle) && !string.IsNullOrWhiteSpace(cx))
            yield return ct => BuscarEnGoogle(consultaWeb, claveGoogle, cx, ct);

        // Siempre disponibles, sin clave
        yield return ct => BuscarEnMapa(texto, codigoPais, ciudad, ct);
        yield return ct => BuscarEnDuckDuckGo(texto, ct);
    }

    private static string ArmarConsulta(string texto, string? ciudad)
    {
        var incluyeCiudad = !string.IsNullOrWhiteSpace(ciudad)
            && texto.Contains(ciudad, StringComparison.OrdinalIgnoreCase);
        return incluyeCiudad || string.IsNullOrWhiteSpace(ciudad)
            ? $"{texto} agencia de turismo"
            : $"{texto} agencia de turismo {ciudad}";
    }

    // ---------- Proveedores con clave ----------

    private async Task<List<AgenciaWebDto>> BuscarEnBrave(string consulta, string clave, string? codigoPais, CancellationToken ct)
    {
        var url = $"https://api.search.brave.com/res/v1/web/search?q={Uri.EscapeDataString(consulta)}&count={MaximoResultados}";
        if (!string.IsNullOrWhiteSpace(codigoPais)) url += $"&country={codigoPais.ToLowerInvariant()}";

        using var pedido = new HttpRequestMessage(HttpMethod.Get, url);
        pedido.Headers.Add("Accept", "application/json");
        pedido.Headers.Add("X-Subscription-Token", clave);

        using var respuesta = await _http.SendAsync(pedido, ct);
        respuesta.EnsureSuccessStatusCode();

        var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        if (!json.TryGetProperty("web", out var web) || !web.TryGetProperty("results", out var items))
            return new List<AgenciaWebDto>();

        return items.EnumerateArray()
            .Select(i => new AgenciaWebDto(
                Texto(i, "title"), Texto(i, "url"), Texto(i, "description"), "web"))
            .Where(a => !string.IsNullOrWhiteSpace(a.Url))
            .ToList();
    }

    private async Task<List<AgenciaWebDto>> BuscarEnGoogle(string consulta, string clave, string cx, CancellationToken ct)
    {
        var url = $"https://www.googleapis.com/customsearch/v1?key={clave}&cx={cx}"
                + $"&q={Uri.EscapeDataString(consulta)}&num={MaximoResultados}";

        var json = await _http.GetFromJsonAsync<JsonElement>(url, ct);
        if (!json.TryGetProperty("items", out var items)) return new List<AgenciaWebDto>();

        return items.EnumerateArray()
            .Select(i => new AgenciaWebDto(
                Texto(i, "title"), Texto(i, "link"), Texto(i, "snippet"), "web"))
            .Where(a => !string.IsNullOrWhiteSpace(a.Url))
            .ToList();
    }

    // ---------- Proveedores sin clave ----------

    /// <summary>Agencias cargadas en OpenStreetMap que tienen sitio web: cubre bien los negocios locales.</summary>
    private async Task<List<AgenciaWebDto>> BuscarEnMapa(string texto, string? codigoPais, string? ciudad, CancellationToken ct)
    {
        var consulta = string.IsNullOrWhiteSpace(ciudad) ? texto : $"{texto}, {ciudad}";
        var url = "https://nominatim.openstreetmap.org/search"
                + $"?q={Uri.EscapeDataString(consulta)}&format=json&limit=15&namedetails=1&extratags=1";
        if (!string.IsNullOrWhiteSpace(codigoPais)) url += $"&countrycodes={codigoPais.ToLowerInvariant()}";

        var json = await _http.GetFromJsonAsync<JsonElement>(url, ct);
        var salida = new List<AgenciaWebDto>();

        foreach (var i in json.EnumerateArray())
        {
            var sitio = SitioWebDe(i);
            if (sitio == null) continue;

            var nombre = i.TryGetProperty("namedetails", out var nd) && nd.ValueKind == JsonValueKind.Object
                ? Texto(nd, "name") : Texto(i, "name");
            if (string.IsNullOrWhiteSpace(nombre)) continue;

            salida.Add(new AgenciaWebDto(nombre, sitio, Texto(i, "display_name"), "mapa"));
        }
        return salida;
    }

    /// <summary>DuckDuckGo sin clave: solo resuelve marcas conocidas (Despegar, Almundo…).</summary>
    private async Task<List<AgenciaWebDto>> BuscarEnDuckDuckGo(string texto, CancellationToken ct)
    {
        var url = $"https://api.duckduckgo.com/?q={Uri.EscapeDataString(texto)}&format=json&no_html=1&skip_disambig=1";
        var json = await _http.GetFromJsonAsync<JsonElement>(url, ct);
        var salida = new List<AgenciaWebDto>();

        var titulo = Texto(json, "Heading");

        // Results trae el sitio oficial; AbstractURL suele apuntar a Wikipedia
        if (json.TryGetProperty("Results", out var items) && items.ValueKind == JsonValueKind.Array)
        {
            foreach (var i in items.EnumerateArray())
            {
                var enlace = Texto(i, "FirstURL");
                if (string.IsNullOrWhiteSpace(enlace)) continue;
                var nombre = Texto(i, "Text");
                // DuckDuckGo devuelve literalmente "Official site": conviene el nombre buscado
                if (string.IsNullOrWhiteSpace(nombre) || nombre.Equals("Official site", StringComparison.OrdinalIgnoreCase))
                    nombre = string.IsNullOrWhiteSpace(titulo) ? texto : titulo;
                salida.Add(new AgenciaWebDto(nombre, enlace, Texto(json, "AbstractText"), "marca"));
            }
        }
        return salida;
    }

    // ---------- Utilidades ----------

    private static string Texto(JsonElement e, string propiedad) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(propiedad, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString() ?? string.Empty
            : string.Empty;

    private static string? SitioWebDe(JsonElement lugar)
    {
        if (!lugar.TryGetProperty("extratags", out var tags) || tags.ValueKind != JsonValueKind.Object) return null;
        foreach (var clave in new[] { "website", "contact:website", "url", "contact:url" })
        {
            var valor = Texto(tags, clave);
            if (string.IsNullOrWhiteSpace(valor)) continue;
            var limpio = valor.Split(';')[0].Trim();
            if (limpio.Length == 0) continue;
            return limpio.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? limpio : $"https://{limpio}";
        }
        return null;
    }

    // Dominios que nunca son el sitio oficial de una agencia
    private static readonly string[] DominiosDescartados =
    {
        "wikipedia.org", "wikidata.org", "wikimedia.org", "wiktionary.org", "wikivoyage.org",
        "facebook.com", "instagram.com", "twitter.com", "x.com", "tripadvisor.com",
        "booking.com", "youtube.com", "linkedin.com",
    };

    private static bool EsDescartado(string dominio) =>
        DominiosDescartados.Any(d => dominio.Equals(d, StringComparison.OrdinalIgnoreCase)
                                  || dominio.EndsWith("." + d, StringComparison.OrdinalIgnoreCase));

    /// <summary>Un mismo sitio puede venir de varias fuentes: se deja una sola entrada por dominio.</summary>
    private static List<AgenciaWebDto> Unificar(List<AgenciaWebDto> resultados)
    {
        var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var salida = new List<AgenciaWebDto>();

        foreach (var a in resultados)
        {
            if (!Uri.TryCreate(a.Url, UriKind.Absolute, out var uri)) continue;
            var dominio = uri.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ? uri.Host[4..] : uri.Host;
            if (EsDescartado(dominio)) continue;
            if (!vistos.Add(dominio)) continue;

            salida.Add(a with { Nombre = string.IsNullOrWhiteSpace(a.Nombre) ? dominio : a.Nombre.Trim() });
            if (salida.Count >= MaximoResultados) break;
        }
        return salida;
    }
}
