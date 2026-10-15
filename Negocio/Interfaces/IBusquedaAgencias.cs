using Negocio.DTOs;

namespace Negocio.Interfaces;

/// <summary>
/// Busca agencias de turismo en la web y devuelve su sitio oficial,
/// para que la publicación pueda mostrarlas como enlace.
/// </summary>
public interface IBusquedaAgencias
{
    Task<List<AgenciaWebDto>> BuscarAsync(
        string consulta, string? codigoPais, string? ciudad, CancellationToken ct = default);
}
