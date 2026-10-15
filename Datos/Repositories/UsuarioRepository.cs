using Datos.Data;
using Microsoft.EntityFrameworkCore;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Datos.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly TrotamundosDbContext _context;

    public UsuarioRepository(TrotamundosDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        var emailNormalizado = email.Trim().ToLower();
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == emailNormalizado);
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int usuarioId)
    {
        return await _context.Usuarios.FindAsync(usuarioId);
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderByDescending(u => u.FechaRegistro)
            .ToListAsync();
    }

    public Task<bool> ExisteSuperAdminAsync() => _context.Usuarios.AnyAsync(u => u.EsSuperAdmin);

    public async Task AgregarUsuarioAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public async Task GuardarTokenRecuperacionAsync(TokenRecuperacion token)
    {
        await _context.TokensRecuperacion.AddAsync(token);
    }

    public async Task<TokenRecuperacion?> ObtenerTokenValidoAsync(string token)
    {
        return await _context.TokensRecuperacion
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Token == token && !t.Usado && t.FechaExpiracion > DateTime.UtcNow);
    }

    public async Task EliminarAsync(int usuarioId)
    {
        // Las publicaciones, reportes y tokens del usuario se borran por cascada (FK).
        // ExecuteDelete evita conflictos con entidades ya trackeadas en este DbContext.
        await _context.Usuarios.Where(u => u.UsuarioId == usuarioId).ExecuteDeleteAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
