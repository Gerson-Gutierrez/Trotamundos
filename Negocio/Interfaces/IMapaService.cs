using Negocio.DTOs;

namespace Negocio.Interfaces;

public interface IMapaService
{
    Task<List<PaisDto>> BuscarPaisesAsync(string? filtro);
    Task<PaisDto?> ObtenerPaisPorCodigoIsoAsync(string codigoIso);
    Task<List<CiudadDto>> ObtenerCiudadesAsync(int paisId, bool soloConPublicaciones, string? filtro = null, int limite = 100);
    Task<CiudadDto> ObtenerOCrearCiudadAsync(CiudadCreateDto dto);
}
