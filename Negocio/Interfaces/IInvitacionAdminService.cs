using Negocio.DTOs;

namespace Negocio.Interfaces;

public interface IInvitacionAdminService
{
    /// <exception cref="InvalidOperationException">Email ya registrado o con invitación activa.</exception>
    /// <exception cref="Negocio.Services.CorreoNoEnviadoException">No se pudo enviar el correo; la invitación se descarta.</exception>
    Task<InvitacionAdminResponseDto> CrearAsync(int adminId, CrearInvitacionAdminDto dto);

    /// <exception cref="InvalidOperationException">Token inválido, expirado o ya consumido.</exception>
    Task AceptarAsync(AceptarInvitacionAdminDto dto);
}
