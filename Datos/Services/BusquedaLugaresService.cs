using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Negocio.DTOs;
using Negocio.Interfaces;

namespace Datos.Services;

/// <summary>
/// Busca alojamientos, restaurantes y agencias por nombre.
///
/// Por qué no alcanza con Nominatim: es un geocodificador de direcciones y
/// devuelve una o dos coincidencias por consulta, no el listado de negocios de
/// una ciudad. Buscar "hotel" en Salta daba 1 resultado.
///
/// Acá se usa Overpass, que consulta los puntos de interés de OpenStreetMap por
/// tipo dentro de un radio: la misma ciudad devuelve ~97 alojamientos, ~169
/// restaurantes y ~25 agencias. Como Overpass limita la cantidad de consultas,
/// se pide una sola vez por ciudad y categoría, se guarda en memoria y el filtro
/// por nombre se resuelve localmente mientras el usuario escribe.
/// </summary>
public class BusquedaLugaresService : IBusquedaLugares
{
    private static readonly TimeSpan DuracionCache = TimeSpan.FromHours(6);
    private const int RadioMetros = 20000;
    private const int MaximoResultados = 12;

    // Espejos de Overpass: si uno está saturado se prueba el siguiente
    private static readonly string[] ServidoresOverpass =
    {
        "https://overpass-api.de/api/interpreter",
        "https://overpass.kumi.systems/api/interpreter",
        "https://overpass.private.coffee/api/interpreter",
    };

    // Etiquetas OSM por categoría de la app
    private static readonly Dictionary<string, (string Filtros, string Etiqueta)> Categorias = new(StringComparer.OrdinalIgnoreCase)
    {
        ["alojamiento"] = (
            "node[\"tourism\"~\"^(hotel|hostel|guest_house|apartment|motel|chalet|resort|camp_site)$\"]({0});" +
            "way[\"tourism\"~\"^(hotel|hostel|guest_house|apartment|motel|chalet|resort|camp_site)$\"]({0});",
            "Alojamiento"),
        ["restaurante"] = (
            "node[\"amenity\"~\"^(restaurant|cafe|bar|fast_food|pub|ice_cream|biergarten)$\"]({0});" +
            "way[\"amenity\"~\"^(restaurant|cafe|bar|fast_food|pub|ice_cream|biergarten)$\"]({0});" +
            "node[\"shop\"~\"^(bakery|pastry|deli|coffee)$\"]({0});",
            "Restaurante"),
        ["agencia"] = (
            "node[\"shop\"=\"travel_agency\"]({0});way[\"shop\"=\"travel_agency\"]({0});" +
            "node[\"office\"=\"travel_agent\"]({0});way[\"office\"=\"travel_agent\"]({0});" +
            "node[\"tourism\"=\"information\"]({0});",
            "Agencia de turismo"),
    };

    private static readonly CompareInfo Comparador = CultureInfo.InvariantCulture.CompareInfo;
    private const CompareOptions SinAcentos = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly ILogger<BusquedaLugaresService> _logger;

    public BusquedaLugaresService(HttpClient http, IMemoryCache cache, ILogger<BusquedaLugaresService> logger)
    {
        _http = http;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>Deja en memoria los lugares de la ciudad para que la primera búsqueda sea instantánea.</summary>
    public async Task<int> PrecargarAsync(string categoria, string? ciudad, string? codigoPais, CancellationToken ct = default)
    {
        if (!Categorias.ContainsKey(categoria)) return 0;
        var lugares = await ObtenerDeLaCiudadAsync(categoria, ciudad, codigoPais, ct);
        return lugares.Count;
    }

    public async Task<List<LugarDto>> BuscarAsync(
        string categoria, string consulta, string? ciudad, string? codigoPais, CancellationToken ct = default)
    {
        var texto = (consulta ?? string.Empty).Trim();
        if (texto.Length < 2 || !Categorias.ContainsKey(categoria)) return new List<LugarDto>();

        var lugares = await ObtenerDeLaCiudadAsync(categoria, ciudad, codigoPais, ct);

        // Si no hay puntos de interés (ciudad desconocida o Overpass caído),
        // se recurre a la geocodificación, que siempre responde algo.
        if (lugares.Count == 0)
            return await BuscarConNominatimAsync(categoria, texto, ciudad, codigoPais, ct);

        var coincidencias = Filtrar(lugares, texto);

        // Pocos resultados propios: se completan con direcciones geocodificadas
        if (coincidencias.Count < 3)
        {
            var extra = await BuscarConNominatimAsync(categoria, texto, ciudad, codigoPais, ct);
            foreach (var l in extra)
            {
                if (coincidencias.Count >= MaximoResultados) break;
                if (!coincidencias.Any(c => c.Nombre.Equals(l.Nombre, StringComparison.OrdinalIgnoreCase)))
                    coincidencias.Add(l);
            }
        }

        return coincidencias;
    }

    // ---------- Puntos de interés de la ciudad (cacheados) ----------

    private async Task<List<LugarDto>> ObtenerDeLaCiudadAsync(
        string categoria, string? ciudad, string? codigoPais, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ciudad)) return new List<LugarDto>();

        var clave = $"poi:{categoria}:{codigoPais}:{ciudad}".ToLowerInvariant();
        if (_cache.TryGetValue(clave, out List<LugarDto>? cacheados) && cacheados != null) return cacheados;

        var centro = await ObtenerCentroCiudadAsync(ciudad, codigoPais, ct);
        if (centro == null) return new List<LugarDto>();

        var lugares = await ConsultarOverpassAsync(categoria, centro.Value.Lat, centro.Value.Lon, ct);

        // Un resultado vacío casi siempre es un servidor saturado, no una ciudad sin
        // lugares: se guarda apenas unos segundos para no repetir la consulta en cada
        // tecla, pero permitiendo reintentar enseguida.
        _cache.Set(clave, lugares, lugares.Count > 0 ? DuracionCache : TimeSpan.FromSeconds(30));
        return lugares;
    }

    private async Task<(double Lat, double Lon)?> ObtenerCentroCiudadAsync(string ciudad, string? codigoPais, CancellationToken ct)
    {
        var clave = $"centro:{codigoPais}:{ciudad}".ToLowerInvariant();
        if (_cache.TryGetValue(clave, out (double, double)? guardado)) return guardado;

        try
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(ciudad)}"
                    + "&format=json&limit=5&addressdetails=1";
            if (!string.IsNullOrWhiteSpace(codigoPais)) url += $"&countrycodes={codigoPais.ToLowerInvariant()}";

            var json = await _http.GetFromJsonAsync<JsonElement>(url, ct);

            // Interesa la ciudad, no la provincia ni el país que a veces salen primero
            var tiposCiudad = new[] { "city", "town", "village", "municipality", "hamlet", "suburb" };
            JsonElement? elegido = null;
            foreach (var r in json.EnumerateArray())
            {
                var tipo = Texto(r, "addresstype");
                if (tiposCiudad.Contains(tipo, StringComparer.OrdinalIgnoreCase)) { elegido = r; break; }
            }
            elegido ??= json.EnumerateArray().Cast<JsonElement?>().FirstOrDefault();
            if (elegido == null) return null;

            if (!double.TryParse(Texto(elegido.Value, "lat"), NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(Texto(elegido.Value, "lon"), NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
                return null;

            var centro = (lat, lon);
            _cache.Set(clave, (ValueTuple<double, double>?)centro, TimeSpan.FromDays(7));
            return centro;
        }
        catch (Exception ex)
        {
            if (ct.IsCancellationRequested) throw;
            _logger.LogWarning(ex, "No se pudo ubicar la ciudad {Ciudad}", ciudad);
            return null;
        }
    }

    private async Task<List<LugarDto>> ConsultarOverpassAsync(string categoria, double lat, double lon, CancellationToken ct)
    {
        var (filtros, etiqueta) = Categorias[categoria];
        var ambito = string.Format(CultureInfo.InvariantCulture, "around:{0},{1},{2}", RadioMetros,
            lat.ToString("F6", CultureInfo.InvariantCulture), lon.ToString("F6", CultureInfo.InvariantCulture));
        var consulta = "[out:json][timeout:25];(" + string.Format(filtros, ambito) + ");out center 300;";

        foreach (var servidor in ServidoresOverpass)
        {
            // Tiempo propio por espejo: si uno tarda, queda margen para el siguiente
            using var limite = CancellationTokenSource.CreateLinkedTokenSource(ct);
            limite.CancelAfter(TimeSpan.FromSeconds(10));

            try
            {
                using var contenido = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("data", consulta) });
                using var respuesta = await _http.PostAsync(servidor, contenido, limite.Token);
                if (!respuesta.IsSuccessStatusCode) continue;

                var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: limite.Token);
                if (!json.TryGetProperty("elements", out var elementos)) continue;

                return Deduplicar(elementos.EnumerateArray()
                    .Select(e => AElemento(e, categoria, etiqueta))
                    .Where(l => l != null)
                    .Select(l => l!));
            }
            catch (Exception ex)
            {
                // Solo se corta si canceló quien pidió la búsqueda; un timeout propio
                // o un espejo saturado no deben frenar los demás intentos.
                if (ct.IsCancellationRequested) throw;
                _logger.LogWarning(ex, "Overpass {Servidor} no respondió", servidor);
            }
        }
        return new List<LugarDto>();
    }

    /// <summary>
    /// Un mismo local suele estar cargado dos veces en OSM (como punto y como
    /// polígono del edificio). Se descarta el repetido si tiene el mismo nombre
    /// y está a menos de 150 metros, quedándose con el que trae más datos.
    /// </summary>
    private static List<LugarDto> Deduplicar(IEnumerable<LugarDto> lugares)
    {
        var salida = new List<LugarDto>();
        foreach (var l in lugares)
        {
            var repetidoEn = salida.FindIndex(x =>
                x.Nombre.Equals(l.Nombre, StringComparison.OrdinalIgnoreCase) && EstanCerca(x, l));

            if (repetidoEn < 0) { salida.Add(l); continue; }
            if (Riqueza(l) > Riqueza(salida[repetidoEn])) salida[repetidoEn] = l;
        }
        return salida;
    }

    private static bool EstanCerca(LugarDto a, LugarDto b)
    {
        if (a.Latitud == null || b.Latitud == null || a.Longitud == null || b.Longitud == null) return true;
        // 150 m ≈ 0,00135 grados de latitud
        return Math.Abs((double)(a.Latitud - b.Latitud)) < 0.00135
            && Math.Abs((double)(a.Longitud - b.Longitud)) < 0.00135;
    }

    private static int Riqueza(LugarDto l) =>
        (l.SitioWeb != null ? 2 : 0) + (l.Direccion != null ? 1 : 0) + (l.Telefono != null ? 1 : 0);

    private static LugarDto? AElemento(JsonElement e, string categoria, string etiqueta)
    {
        if (!e.TryGetProperty("tags", out var tags) || tags.ValueKind != JsonValueKind.Object) return null;
        var nombre = Texto(tags, "name");
        if (string.IsNullOrWhiteSpace(nombre)) return null;

        decimal? lat = null, lon = null;
        if (e.TryGetProperty("lat", out var la) && la.ValueKind == JsonValueKind.Number) lat = la.GetDecimal();
        if (e.TryGetProperty("lon", out var lo) && lo.ValueKind == JsonValueKind.Number) lon = lo.GetDecimal();
        if (lat == null && e.TryGetProperty("center", out var centro) && centro.ValueKind == JsonValueKind.Object)
        {
            if (centro.TryGetProperty("lat", out var cla)) lat = cla.GetDecimal();
            if (centro.TryGetProperty("lon", out var clo)) lon = clo.GetDecimal();
        }

        return new LugarDto(
            nombre,
            DireccionDe(tags),
            lat, lon,
            SitioWebDe(tags),
            Primero(tags, "phone", "contact:phone"),
            categoria,
            SubEtiqueta(tags) ?? etiqueta,
            "poi");
    }

    private static string? DireccionDe(JsonElement tags)
    {
        var calle = Texto(tags, "addr:street");
        var numero = Texto(tags, "addr:housenumber");
        var ciudad = Texto(tags, "addr:city");
        var partes = new[]
        {
            string.IsNullOrWhiteSpace(numero) ? calle : $"{calle} {numero}".Trim(),
            ciudad,
        }.Where(p => !string.IsNullOrWhiteSpace(p));
        var direccion = string.Join(", ", partes);
        return string.IsNullOrWhiteSpace(direccion) ? null : direccion;
    }

    private static string? SubEtiqueta(JsonElement tags)
    {
        var mapa = new Dictionary<string, string>
        {
            ["hotel"] = "Hotel", ["hostel"] = "Hostel", ["guest_house"] = "Casa de huéspedes",
            ["apartment"] = "Departamento", ["motel"] = "Motel", ["chalet"] = "Cabaña",
            ["resort"] = "Resort", ["camp_site"] = "Camping",
            ["restaurant"] = "Restaurante", ["cafe"] = "Café", ["bar"] = "Bar",
            ["fast_food"] = "Comida rápida", ["pub"] = "Pub", ["ice_cream"] = "Heladería",
            ["bakery"] = "Panadería", ["travel_agency"] = "Agencia de viajes",
            ["travel_agent"] = "Agencia de viajes", ["information"] = "Oficina de turismo",
        };
        foreach (var clave in new[] { "tourism", "amenity", "shop", "office" })
        {
            var valor = Texto(tags, clave);
            if (!string.IsNullOrWhiteSpace(valor) && mapa.TryGetValue(valor, out var etiqueta)) return etiqueta;
        }
        return null;
    }

    // ---------- Respaldo por geocodificación ----------

    private async Task<List<LugarDto>> BuscarConNominatimAsync(
        string categoria, string texto, string? ciudad, string? codigoPais, CancellationToken ct)
    {
        try
        {
            var consulta = string.IsNullOrWhiteSpace(ciudad) || texto.Contains(ciudad, StringComparison.OrdinalIgnoreCase)
                ? texto : $"{texto}, {ciudad}";
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(consulta)}"
                    + "&format=json&limit=8&addressdetails=1&namedetails=1&extratags=1";
            if (!string.IsNullOrWhiteSpace(codigoPais)) url += $"&countrycodes={codigoPais.ToLowerInvariant()}";

            var json = await _http.GetFromJsonAsync<JsonElement>(url, ct);
            var salida = new List<LugarDto>();

            foreach (var r in json.EnumerateArray())
            {
                var nombre = r.TryGetProperty("namedetails", out var nd) && nd.ValueKind == JsonValueKind.Object
                    ? Texto(nd, "name") : Texto(r, "name");
                var display = Texto(r, "display_name");
                if (string.IsNullOrWhiteSpace(nombre)) nombre = display;
                if (string.IsNullOrWhiteSpace(nombre)) continue;

                decimal.TryParse(Texto(r, "lat"), NumberStyles.Any, CultureInfo.InvariantCulture, out var lat);
                decimal.TryParse(Texto(r, "lon"), NumberStyles.Any, CultureInfo.InvariantCulture, out var lon);

                var tags = r.TryGetProperty("extratags", out var et) && et.ValueKind == JsonValueKind.Object ? et : default;

                var etiqueta = EtiquetaNominatim(r, categoria, out var esDeLaCategoria);

                salida.Add(new LugarDto(
                    nombre,
                    display.StartsWith(nombre + ", ", StringComparison.Ordinal) ? display[(nombre.Length + 2)..] : display,
                    lat, lon,
                    tags.ValueKind == JsonValueKind.Object ? SitioWebDe(tags) : null,
                    null,
                    categoria,
                    etiqueta,
                    esDeLaCategoria ? "poi" : "direccion"));
            }

            // Un hotel encontrado vale más que una calle con el mismo nombre
            return salida.OrderByDescending(l => l.Fuente == "poi").ToList();
        }
        catch (Exception ex)
        {
            if (ct.IsCancellationRequested) throw;
            _logger.LogWarning(ex, "Falló la geocodificación de respaldo");
            return new List<LugarDto>();
        }
    }

    /// <summary>Clasifica un resultado de geocodificación según su etiqueta OSM.</summary>
    private static string EtiquetaNominatim(JsonElement r, string categoria, out bool esDeLaCategoria)
    {
        var clase = Texto(r, "class");
        var tipo = Texto(r, "type");

        var tiposPorCategoria = new Dictionary<string, string[]>
        {
            ["alojamiento"] = new[] { "hotel", "hostel", "guest_house", "apartment", "motel", "chalet", "resort", "camp_site" },
            ["restaurante"] = new[] { "restaurant", "cafe", "bar", "fast_food", "pub", "ice_cream", "bakery" },
            ["agencia"] = new[] { "travel_agency", "travel_agent", "information" },
        };

        esDeLaCategoria = tiposPorCategoria.TryGetValue(categoria, out var tipos)
            && tipos.Contains(tipo, StringComparer.OrdinalIgnoreCase);

        if (esDeLaCategoria)
        {
            var tags = JsonDocument.Parse($"{{\"{clase}\":\"{tipo}\"}}").RootElement;
            return SubEtiqueta(tags) ?? Categorias[categoria].Etiqueta;
        }
        return "Dirección";
    }

    // ---------- Filtrado por nombre ----------

    private static List<LugarDto> Filtrar(List<LugarDto> lugares, string texto)
    {
        // Todas las palabras escritas deben aparecer en el nombre: "hotel sol"
        // encuentra "Hotel del Sol" sin traer todos los hoteles.
        var palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return lugares
            .Where(l => palabras.All(p => Contiene(l.Nombre, p)))
            .OrderByDescending(l => Empieza(l.Nombre, palabras[0]))
            .ThenByDescending(l => l.SitioWeb != null)
            .ThenBy(l => l.Nombre.Length)
            .Take(MaximoResultados)
            .ToList();
    }

    private static bool Contiene(string texto, string busqueda) =>
        Comparador.IndexOf(texto, busqueda, SinAcentos) >= 0;

    private static bool Empieza(string texto, string busqueda) =>
        Comparador.IsPrefix(texto, busqueda, SinAcentos);

    // ---------- Utilidades ----------

    private static string Texto(JsonElement e, string propiedad) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(propiedad, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString() ?? string.Empty
            : string.Empty;

    private static string? Primero(JsonElement tags, params string[] claves)
    {
        foreach (var c in claves)
        {
            var v = Texto(tags, c);
            if (!string.IsNullOrWhiteSpace(v)) return v;
        }
        return null;
    }

    private static string? SitioWebDe(JsonElement tags)
    {
        var url = Primero(tags, "website", "contact:website", "url", "contact:url");
        if (url == null) return null;
        var limpio = url.Split(';')[0].Trim();
        if (limpio.Length == 0) return null;
        return limpio.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? limpio : $"https://{limpio}";
    }
}
