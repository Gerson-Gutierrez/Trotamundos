using Negocio.DTOs;
using Negocio.Entidades;

namespace Negocio.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegistroDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<bool> SolicitarRecuperacionPasswordAsync(string email);
    Task<bool> ResetearPasswordAsync(CambiarPasswordDto dto);

    /// <summary>Genera un token de recuperación para la cuenta y lo envía a su correo.</summary>
    Task EnviarRecuperacionAsync(Usuario usuario);
    Task<UsuarioPerfilDto?> ObtenerPerfilAsync(int usuarioId);
    Task<UsuarioPerfilDto> ActualizarPerfilAsync(int usuarioId, ActualizarPerfilDto dto);

    /// <summary>Baja definitiva de la cuenta del propio usuario (requiere confirmar la contraseña).</summary>
    Task EliminarCuentaAsync(int usuarioId, string password);
}
