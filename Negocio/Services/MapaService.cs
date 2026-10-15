using System.Globalization;
using Negocio.DTOs;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Negocio.Services;

public class MapaService : IMapaService
{
    private readonly IMapaRepository _mapaRepository;
    private readonly ICatalogoCiudades _catalogo;

    public MapaService(IMapaRepository mapaRepository, ICatalogoCiudades catalogo)
    {
        _mapaRepository = mapaRepository;
        _catalogo = catalogo;
    }

    public async Task<List<PaisDto>> BuscarPaisesAsync(string? filtro)
    {
        var paises = await _mapaRepository.ObtenerPaisesAsync(filtro);
        return paises.Select(MapearPais).ToList();
    }

    public async Task<PaisDto?> ObtenerPaisPorCodigoIsoAsync(string codigoIso)
    {
        var pais = await _mapaRepository.ObtenerPaisPorCodigoIsoAsync(codigoIso);
        return pais == null ? null : MapearPais(pais);
    }

    /// <summary>
    /// Ciudades de un país para el desplegable. Une las ya registradas (las que
    /// tienen publicaciones) con el catálogo mundial, para que el listado no
    /// dependa de que alguien haya publicado antes en ese país.
    /// Con soloConPublicaciones = true devuelve únicamente las de la base
    /// (es el filtro del inicio y no debe ofrecer ciudades sin contenido).
    /// </summary>
    public async Task<List<CiudadDto>> ObtenerCiudadesAsync(
        int paisId, bool soloConPublicaciones, string? filtro = null, int limite = 100)
    {
        var registradas = await _mapaRepository.ObtenerCiudadesPorPaisAsync(paisId, soloConPublicaciones);

        // Una ciudad puede estar registrada sin tener publicaciones visibles todavía
        var conContenido = soloConPublicaciones
            ? registradas.Select(c => c.CiudadId).ToHashSet()
            : (await _mapaRepository.ObtenerCiudadesPorPaisAsync(paisId, true)).Select(c => c.CiudadId).ToHashSet();

        var resultado = registradas
            .Select(c => new CiudadDto(
                c.CiudadId, c.PaisId, c.Nombre, c.PlaceIdApi,
                Registrada: true, TienePublicaciones: conContenido.Contains(c.CiudadId)))
            .ToList();

        if (soloConPublicaciones)
            return Filtrar(resultado, filtro, limite);

        var pais = await _mapaRepository.ObtenerPaisPorIdAsync(paisId);
        if (pais?.CodigoIso != null)
        {
            var yaEstan = new HashSet<string>(resultado.Select(c => c.Nombre), StringComparer.OrdinalIgnoreCase);
            foreach (var nombre in _catalogo.ObtenerPorIso(pais.CodigoIso))
            {
                if (yaEstan.Add(nombre))
                    resultado.Add(new CiudadDto(0, paisId, nombre, null, Registrada: false, TienePublicaciones: false));
            }
        }

        // Las registradas primero (ya tienen contenido), después el resto alfabético
        resultado = resultado
            .OrderByDescending(c => c.TienePublicaciones)
            .ThenByDescending(c => c.Registrada)
            .ThenBy(c => c.Nombre, StringComparer.CurrentCulture)
            .ToList();

        return Filtrar(resultado, filtro, limite);
    }

    // Búsqueda que ignora mayúsculas y acentos: escribir "cordo" tiene que
    // encontrar "Córdoba", y "jujuy" tiene que encontrar "Jujuy".
    private static readonly CompareInfo Comparador = CultureInfo.InvariantCulture.CompareInfo;
    private const CompareOptions OpcionesBusqueda = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    private static bool Contiene(string texto, string busqueda) =>
        Comparador.IndexOf(texto, busqueda, OpcionesBusqueda) >= 0;

    private static bool Empieza(string texto, string busqueda) =>
        Comparador.IsPrefix(texto, busqueda, OpcionesBusqueda);

    private static List<CiudadDto> Filtrar(List<CiudadDto> ciudades, string? filtro, int limite)
    {
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.Trim();
            ciudades = ciudades
                .Where(c => Contiene(c.Nombre, texto))
                // Las que empiezan con el texto buscado van primero
                .OrderByDescending(c => Empieza(c.Nombre, texto))
                .ThenByDescending(c => c.TienePublicaciones)
                .ThenBy(c => c.Nombre, StringComparer.CurrentCulture)
                .ToList();
        }
        return limite > 0 ? ciudades.Take(limite).ToList() : ciudades;
    }

    public async Task<CiudadDto> ObtenerOCrearCiudadAsync(CiudadCreateDto dto)
    {
        var pais = await _mapaRepository.ObtenerPaisPorIdAsync(dto.PaisId)
            ?? throw new KeyNotFoundException("El país indicado no existe.");

        var existente = await _mapaRepository.ObtenerCiudadPorNombreAsync(pais.PaisId, dto.Nombre);
        if (existente != null)
        {
            // Completa el PlaceId si la ciudad ya existía sin él
            if (string.IsNullOrEmpty(existente.PlaceIdApi) && !string.IsNullOrEmpty(dto.PlaceIdApi))
            {
                existente.PlaceIdApi = dto.PlaceIdApi;
                await _mapaRepository.ActualizarCiudadAsync(existente);
            }
            return MapearCiudad(existente);
        }

        var nueva = await _mapaRepository.CrearCiudadAsync(new Ciudad
        {
            PaisId = pais.PaisId,
            Nombre = dto.Nombre.Trim(),
            PlaceIdApi = dto.PlaceIdApi
        });

        return MapearCiudad(nueva);
    }

    private static PaisDto MapearPais(Pais p) => new(p.PaisId, p.Nombre, p.CodigoIso);
    private static CiudadDto MapearCiudad(Ciudad c) => new(c.CiudadId, c.PaisId, c.Nombre, c.PlaceIdApi, Registrada: true, TienePublicaciones: true);
}
