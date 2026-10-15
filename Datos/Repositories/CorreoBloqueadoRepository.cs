using Datos.Data;
using Microsoft.EntityFrameworkCore;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Datos.Repositories;

public class CorreoBloqueadoRepository : ICorreoBloqueadoRepository
{
    private readonly TrotamundosDbContext _context;

    public CorreoBloqueadoRepository(TrotamundosDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteAsync(string email)
    {
        var normalizado = email.Trim().ToLower();
        return _context.CorreosBloqueados.AnyAsync(c => c.Email == normalizado);
    }

    public Task<List<CorreoBloqueado>> ListarAsync()
        => _context.CorreosBloqueados.AsNoTracking().OrderByDescending(c => c.FechaBloqueo).ToListAsync();

    public async Task<bool> QuitarAsync(string email)
    {
        var normalizado = email.Trim().ToLower();
        return await _context.CorreosBloqueados.Where(c => c.Email == normalizado).ExecuteDeleteAsync() > 0;
    }

    public async Task EliminarUsuarioYBloquearCorreoAsync(int usuarioId, CorreoBloqueado bloqueo)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        // Las publicaciones, reportes, tokens e invitaciones de la cuenta se borran por cascada (FK)
        await _context.Usuarios.Where(u => u.UsuarioId == usuarioId).ExecuteDeleteAsync();

        if (!await _context.CorreosBloqueados.AnyAsync(c => c.Email == bloqueo.Email))
        {
            _context.CorreosBloqueados.Add(bloqueo);
            await _context.SaveChangesAsync();
        }

        await tx.CommitAsync();
    }
}
