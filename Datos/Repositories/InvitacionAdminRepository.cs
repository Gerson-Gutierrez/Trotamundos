using Datos.Data;
using Microsoft.EntityFrameworkCore;
using Negocio.Entidades;
using Negocio.Interfaces;

namespace Datos.Repositories;

public class InvitacionAdminRepository : IInvitacionAdminRepository
{
    private readonly TrotamundosDbContext _context;

    public InvitacionAdminRepository(TrotamundosDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteActivaPorEmailAsync(string email)
        => _context.InvitacionesAdmin
            .AnyAsync(i => i.Email == email && !i.Usada && i.FechaExpiracion > DateTime.UtcNow);

    public async Task AgregarAsync(InvitacionAdmin invitacion)
    {
        _context.InvitacionesAdmin.Add(invitacion);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int invitacionId)
        => await _context.InvitacionesAdmin.Where(i => i.InvitacionId == invitacionId).ExecuteDeleteAsync();

    public Task<InvitacionAdmin?> ObtenerPorTokenHashAsync(string tokenHash)
        => _context.InvitacionesAdmin.AsNoTracking().FirstOrDefaultAsync(i => i.TokenHash == tokenHash);

    public async Task<bool> ConsumirYCrearUsuarioAsync(int invitacionId, Usuario usuario)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        // UPDATE condicional: solo una petición concurrente puede marcarla como usada
        var filas = await _context.InvitacionesAdmin
            .Where(i => i.InvitacionId == invitacionId && !i.Usada && i.FechaExpiracion > DateTime.UtcNow)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.Usada, true));

        if (filas == 0) return false;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        return true;
    }
}
