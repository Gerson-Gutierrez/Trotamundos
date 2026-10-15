using System.Text.Json;
using Microsoft.Extensions.Logging;
using Negocio.Interfaces;

namespace Datos.Services;

/// <summary>
/// Lee el catálogo mundial de ciudades (Data/ciudades-mundo.json, ~150.000 ciudades
/// de 223 países). Se carga una sola vez en memoria la primera vez que se pide.
/// Registrar como singleton.
/// </summary>
public class CatalogoCiudadesService : ICatalogoCiudades
{
    private const string NombreArchivo = "Data/ciudades-mundo.json";

    private readonly ILogger<CatalogoCiudadesService> _logger;
    private readonly Lazy<Dictionary<string, string[]>> _catalogo;

    public CatalogoCiudadesService(ILogger<CatalogoCiudadesService> logger)
    {
        _logger = logger;
        _catalogo = new Lazy<Dictionary<string, string[]>>(Cargar, isThreadSafe: true);
    }

    public IReadOnlyList<string> ObtenerPorIso(string? codigoIso)
    {
        if (string.IsNullOrWhiteSpace(codigoIso)) return Array.Empty<string>();
        return _catalogo.Value.TryGetValue(codigoIso.Trim().ToUpperInvariant(), out var ciudades)
            ? ciudades
            : Array.Empty<string>();
    }

    private Dictionary<string, string[]> Cargar()
    {
        var ruta = Path.Combine(AppContext.BaseDirectory, NombreArchivo);
        try
        {
            using var stream = File.OpenRead(ruta);
            var datos = JsonSerializer.Deserialize<Dictionary<string, string[]>>(stream)
                        ?? new Dictionary<string, string[]>();
            _logger.LogInformation("Catálogo de ciudades cargado: {Paises} países", datos.Count);
            return new Dictionary<string, string[]>(datos, StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            // Sin catálogo la app sigue andando: solo se ofrecen las ciudades ya registradas
            _logger.LogError(ex, "No se pudo cargar el catálogo de ciudades desde {Ruta}", ruta);
            return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
