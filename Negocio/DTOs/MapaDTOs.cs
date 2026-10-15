using System.ComponentModel.DataAnnotations;

namespace Negocio.DTOs;

public record PaisDto(int PaisId, string Nombre, string? CodigoIso);

// CiudadId = 0 cuando la ciudad viene del catálogo mundial y todavía no se registró.
// Registrada: ya existe en la base. TienePublicaciones: además tiene contenido visible.
public record CiudadDto(
    int CiudadId, int PaisId, string Nombre, string? PlaceIdApi,
    bool Registrada = false, bool TienePublicaciones = false);

// Alta de ciudad desde la app (find-or-create): si ya existe una con el mismo
// nombre en ese país se devuelve la existente en vez de duplicarla.
public record CiudadCreateDto(
    [Range(1, int.MaxValue)] int PaisId,
    [Required, MaxLength(150)] string Nombre,
    string? PlaceIdApi);
