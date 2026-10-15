using Negocio.Entidades;

namespace Negocio.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<Usuario?> ObtenerPorIdAsync(int usuarioId);
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<bool> ExisteSuperAdminAsync();
    Task AgregarUsuarioAsync(Usuario usuario);
    Task GuardarTokenRecuperacionAsync(TokenRecuperacion token);
    Task<TokenRecuperacion?> ObtenerTokenValidoAsync(string token);
    Task EliminarAsync(int usuarioId);
    Task GuardarCambiosAsync();
}
