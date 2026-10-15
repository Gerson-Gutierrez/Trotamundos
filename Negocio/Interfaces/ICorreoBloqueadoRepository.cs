using Negocio.Entidades;

namespace Negocio.Interfaces;

public interface ICorreoBloqueadoRepository
{
    Task<bool> ExisteAsync(string email);
    Task<List<CorreoBloqueado>> ListarAsync();
    Task<bool> QuitarAsync(string email);

    /// <summary>Elimina la cuenta y veta su correo en una sola transacción.</summary>
    Task EliminarUsuarioYBloquearCorreoAsync(int usuarioId, CorreoBloqueado bloqueo);
}
