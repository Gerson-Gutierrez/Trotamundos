using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Negocio.DTOs;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Negocio.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly ICorreoBloqueadoRepository _correosBloqueados;
    private readonly IConfiguration _config;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        ICorreoBloqueadoRepository correosBloqueados,
        IConfiguration config)
    {
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
        _correosBloqueados = correosBloqueados;
        _config = config;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistroDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(email);
        if (usuarioExistente != null)
            throw new InvalidOperationException("El email ya se encuentra registrado.");

        // El correo del superadministrador inicial está reservado: lo crea el arranque, no el registro público
        if (email == _config["SuperAdmin:Email"]?.Trim().ToLower())
            throw new InvalidOperationException("El email ya se encuentra registrado.");

        if (await _correosBloqueados.ExisteAsync(email))
            throw new InvalidOperationException("Este correo electrónico no puede utilizarse para registrarse.");

        // Opcional: si viene, tiene que ser un usuario válido (lanza ArgumentException si no)
        var instagram = InstagramHelper.Normalizar(dto.Instagram);

        PasswordHasher.Crear(dto.Password, out byte[] hash, out byte[] salt);

        // El primer administrador se define por configuración (Admin:Email);
        // el resto de los usuarios se registran como viajeros comunes.
        var adminEmail = _config["Admin:Email"]?.Trim().ToLower();

        var usuario = new Usuario
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Nacionalidad = dto.Nacionalidad.Trim(),
            FotoPerfilUrl = dto.FotoPerfilUrl,
            InstagramUsuario = instagram,
            EsAdmin = !string.IsNullOrEmpty(adminEmail) && adminEmail == email
        };

        await _usuarioRepository.AgregarUsuarioAsync(usuario);
        await _usuarioRepository.GuardarCambiosAsync();

        return CrearRespuesta(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);
        if (usuario == null || !PasswordHasher.Verificar(dto.Password, usuario.PasswordHash, usuario.PasswordSalt))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (usuario.Bloqueado)
            throw new UnauthorizedAccessException("La cuenta se encuentra bloqueada. Contactá al administrador.");

        return CrearRespuesta(usuario);
    }

    public async Task<bool> SolicitarRecuperacionPasswordAsync(string email)
    {
        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
        if (usuario == null) return false;

        // La recuperación de cuentas administrativas la gestiona un superadministrador
        if (usuario.EsAdmin || usuario.EsSuperAdmin) return false;

        // El endpoint público siempre responde igual para no revelar qué correos existen;
        // el fallo de envío ya queda registrado en el log por EmailService.
        try { await EnviarRecuperacionAsync(usuario); }
        catch (CorreoNoEnviadoException) { return false; }
        return true;
    }

    public async Task EnviarRecuperacionAsync(Usuario usuario)
    {
        var tokenRegistro = new TokenRecuperacion
        {
            UsuarioId = usuario.UsuarioId,
            Token = Guid.NewGuid().ToString("N"),
            FechaExpiracion = DateTime.UtcNow.AddHours(2)
        };

        await _usuarioRepository.GuardarTokenRecuperacionAsync(tokenRegistro);
        await _usuarioRepository.GuardarCambiosAsync();

        await _emailService.EnviarEmailRecuperacionAsync(usuario.Email, tokenRegistro.Token);
    }

    public async Task<bool> ResetearPasswordAsync(CambiarPasswordDto dto)
    {
        var tokenObj = await _usuarioRepository.ObtenerTokenValidoAsync(dto.Token);
        if (tokenObj == null) return false;

        PasswordHasher.Crear(dto.NuevaPassword, out byte[] hash, out byte[] salt);
        tokenObj.Usuario.PasswordHash = hash;
        tokenObj.Usuario.PasswordSalt = salt;
        tokenObj.Usado = true;

        await _usuarioRepository.GuardarCambiosAsync();
        return true;
    }

    public async Task<UsuarioPerfilDto?> ObtenerPerfilAsync(int usuarioId)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
        return usuario == null ? null : MapearPerfil(usuario);
    }

    public async Task<UsuarioPerfilDto> ActualizarPerfilAsync(int usuarioId, ActualizarPerfilDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("El usuario no existe.");

        usuario.Nombre = dto.Nombre.Trim();
        usuario.Apellido = dto.Apellido.Trim();
        usuario.Nacionalidad = dto.Nacionalidad.Trim();
        usuario.FotoPerfilUrl = dto.FotoPerfilUrl;
        usuario.InstagramUsuario = InstagramHelper.Normalizar(dto.Instagram);

        await _usuarioRepository.GuardarCambiosAsync();
        return MapearPerfil(usuario);
    }

    public async Task EliminarCuentaAsync(int usuarioId, string password)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("El usuario no existe.");

        // Las cuentas administrativas se gestionan desde el panel del superadministrador
        if (usuario.EsAdmin || usuario.EsSuperAdmin)
            throw new InvalidOperationException("Las cuentas de administrador no pueden darse de baja desde aquí.");

        if (!PasswordHasher.Verificar(password, usuario.PasswordHash, usuario.PasswordSalt))
            throw new UnauthorizedAccessException("La contraseña es incorrecta.");

        // Publicaciones, reportes y tokens se eliminan por cascada
        await _usuarioRepository.EliminarAsync(usuarioId);
    }

    private AuthResponseDto CrearRespuesta(Usuario usuario) => new(
        usuario.UsuarioId, usuario.Nombre, usuario.Apellido, usuario.Email,
        usuario.Nacionalidad, usuario.FotoPerfilUrl, usuario.EsAdmin, usuario.EsSuperAdmin, GenerarJwtToken(usuario),
        usuario.InstagramUsuario);

    private static UsuarioPerfilDto MapearPerfil(Usuario u) => new(
        u.UsuarioId, u.Nombre, u.Apellido, u.Email, u.Nacionalidad, u.FotoPerfilUrl, u.EsAdmin, u.EsSuperAdmin, u.FechaRegistro, u.InstagramUsuario);

    private string GenerarJwtToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.EsSuperAdmin ? "SuperAdmin" : usuario.EsAdmin ? "Admin" : "User")
        };

        var secret = _config["Jwt:Secret"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:Secret.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
