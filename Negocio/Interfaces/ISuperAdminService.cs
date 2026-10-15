using Negocio.DTOs;

namespace Negocio.Interfaces;

public interface ISuperAdminService
{
    Task<List<CuentaAdministrativaDto>> ListarCuentasAsync();

    /// <summary>Elimina a un administrador común y bloquea su correo. Los superadministradores no se pueden eliminar.</summary>
    /// <exception cref="KeyNotFoundException">La cuenta no existe.</exception>
    /// <exception cref="InvalidOperationException">La cuenta no es un administrador común.</exception>
    Task EliminarAdministradorAsync(int superAdminId, int usuarioId, string? motivo);

    /// <summary>Envía al correo de un administrador o superadministrador un código para elegir nueva contraseña.</summary>
    /// <exception cref="KeyNotFoundException">La cuenta no existe.</exception>
    /// <exception cref="InvalidOperationException">La cuenta no es administrativa.</exception>
    Task RecuperarPasswordAsync(int usuarioId);

    Task<List<CorreoBloqueadoDto>> ListarCorreosBloqueadosAsync();
    Task<bool> DesbloquearCorreoAsync(string email);

    /// <summary>Crea (o promueve) el primer superadministrador definido en configuración si todavía no existe ninguno.</summary>
    Task AsegurarSuperAdminInicialAsync();
}
