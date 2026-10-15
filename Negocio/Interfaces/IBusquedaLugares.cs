using Negocio.DTOs;

namespace Negocio.Interfaces;

/// <summary>
/// Busca alojamientos, restaurantes y agencias por nombre dentro de una ciudad.
/// </summary>
public interface IBusquedaLugares
{
    /// <param name="categoria">alojamiento | restaurante | agencia</param>
    Task<List<LugarDto>> BuscarAsync(
        string categoria, string consulta, string? ciudad, string? codigoPais, CancellationToken ct = default);

    /// <summary>Carga por adelantado los lugares de la ciudad. Devuelve cuántos quedaron listos.</summary>
    Task<int> PrecargarAsync(string categoria, string? ciudad, string? codigoPais, CancellationToken ct = default);
}
