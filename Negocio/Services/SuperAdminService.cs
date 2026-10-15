using Microsoft.Extensions.Configuration;
using Negocio.DTOs;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Negocio.Services;

public class SuperAdminService : ISuperAdminService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ICorreoBloqueadoRepository _correosBloqueados;
    private readonly IAuthService _auth;
    private readonly IConfiguration _config;

    public SuperAdminService(
        IUsuarioRepository usuarios,
        ICorreoBloqueadoRepository correosBloqueados,
        IAuthService auth,
        IConfiguration config)
    {
        _usuarios = usuarios;
        _correosBloqueados = correosBloqueados;
        _auth = auth;
        _config = config;
    }

    public async Task<List<CuentaAdministrativaDto>> ListarCuentasAsync()
    {
        var usuarios = await _usuarios.ObtenerTodosAsync();
        return usuarios
            .Where(u => u.EsAdmin || u.EsSuperAdmin)
            .Select(u => new CuentaAdministrativaDto(
                u.UsuarioId, u.Nombre, u.Apellido, u.Email, u.EsSuperAdmin, u.Bloqueado, u.FechaRegistro))
            .ToList();
    }

    public async Task EliminarAdministradorAsync(int superAdminId, int usuarioId, string? motivo)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("La cuenta no existe.");

        if (usuario.EsSuperAdmin)
            throw new InvalidOperationException("No se puede eliminar a un superadministrador.");
        if (!usuario.EsAdmin)
            throw new InvalidOperationException("La cuenta no es un administrador.");

        var bloqueo = new CorreoBloqueado
        {
            Email = usuario.Email.Trim().ToLower(),
            Motivo = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim(),
            BloqueadoPorUsuarioId = superAdminId
        };

        await _correosBloqueados.EliminarUsuarioYBloquearCorreoAsync(usuarioId, bloqueo);
    }

    public async Task RecuperarPasswordAsync(int usuarioId)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("La cuenta no existe.");

        if (!usuario.EsAdmin && !usuario.EsSuperAdmin)
            throw new InvalidOperationException("Solo se puede recuperar la contraseña de cuentas administrativas.");

        await _auth.EnviarRecuperacionAsync(usuario);
    }

    public async Task<List<CorreoBloqueadoDto>> ListarCorreosBloqueadosAsync()
    {
        var bloqueados = await _correosBloqueados.ListarAsync();
        return bloqueados.Select(c => new CorreoBloqueadoDto(c.Email, c.Motivo, c.FechaBloqueo)).ToList();
    }

    public Task<bool> DesbloquearCorreoAsync(string email) => _correosBloqueados.QuitarAsync(email);

    public async Task AsegurarSuperAdminInicialAsync()
    {
        var email = _config["SuperAdmin:Email"]?.Trim().ToLower();
        var password = _config["SuperAdmin:Password"];
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return;

        if (await _usuarios.ExisteSuperAdminAsync()) return;

        PasswordHasher.Crear(password, out var hash, out var salt);

        var existente = await _usuarios.ObtenerPorEmailAsync(email);
        if (existente != null)
        {
            existente.EsSuperAdmin = true;
            existente.EsAdmin = false;
            existente.Bloqueado = false;
            existente.PasswordHash = hash;
            existente.PasswordSalt = salt;
        }
        else
        {
            await _usuarios.AgregarUsuarioAsync(new Usuario
            {
                Nombre = "Super",
                Apellido = "Administrador",
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                EsSuperAdmin = true
            });
        }

        await _usuarios.GuardarCambiosAsync();
    }
}
