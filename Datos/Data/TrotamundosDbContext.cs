using Microsoft.EntityFrameworkCore;
using Negocio.Entidades;

namespace Datos.Data;

public class TrotamundosDbContext : DbContext
{
    public TrotamundosDbContext(DbContextOptions<TrotamundosDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<TokenRecuperacion> TokensRecuperacion => Set<TokenRecuperacion>();
    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();
    public DbSet<Publicacion> Publicaciones => Set<Publicacion>();
    public DbSet<Alojamiento> Alojamientos => Set<Alojamiento>();
    public DbSet<ExperienciaGastronomica> ExperienciasGastronomicas => Set<ExperienciaGastronomica>();
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Recomendacion> Recomendaciones => Set<Recomendacion>();
    public DbSet<AgenciaTurismo> AgenciasTurismo => Set<AgenciaTurismo>();
    public DbSet<PublicacionImagen> PublicacionImagenes => Set<PublicacionImagen>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<NotificacionAdmin> NotificacionesAdmin => Set<NotificacionAdmin>();
    public DbSet<InvitacionAdmin> InvitacionesAdmin => Set<InvitacionAdmin>();
    public DbSet<CorreoBloqueado> CorreosBloqueados => Set<CorreoBloqueado>();
    public DbSet<PublicacionFavorita> PublicacionesFavoritas => Set<PublicacionFavorita>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // MAPEO DE TABLAS
        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<TokenRecuperacion>().ToTable("tokens_recuperacion");
        modelBuilder.Entity<Pais>().ToTable("paises");
        modelBuilder.Entity<Ciudad>().ToTable("ciudades");
        modelBuilder.Entity<Publicacion>().ToTable("publicaciones");
        modelBuilder.Entity<Alojamiento>().ToTable("alojamientos");
        modelBuilder.Entity<ExperienciaGastronomica>().ToTable("experiencias_gastronomicas");
        modelBuilder.Entity<Restaurante>().ToTable("restaurantes");
        modelBuilder.Entity<Recomendacion>().ToTable("recomendaciones");
        modelBuilder.Entity<AgenciaTurismo>().ToTable("agencias_turismo");
        modelBuilder.Entity<PublicacionImagen>().ToTable("publicacion_imagenes");
        modelBuilder.Entity<Reporte>().ToTable("reportes");
        modelBuilder.Entity<NotificacionAdmin>().ToTable("notificaciones_admin");
        modelBuilder.Entity<InvitacionAdmin>().ToTable("invitaciones_admin");
        modelBuilder.Entity<CorreoBloqueado>().ToTable("correos_bloqueados");
        modelBuilder.Entity<PublicacionFavorita>().ToTable("publicaciones_favoritas");

        // --- CLAVES PRIMARIAS (PRIMARY KEYS) ---
        modelBuilder.Entity<Usuario>().HasKey(u => u.UsuarioId);
        modelBuilder.Entity<TokenRecuperacion>().HasKey(t => t.TokenId);
        modelBuilder.Entity<Pais>().HasKey(p => p.PaisId);
        modelBuilder.Entity<Ciudad>().HasKey(c => c.CiudadId);
        modelBuilder.Entity<Publicacion>().HasKey(p => p.PublicacionId);
        modelBuilder.Entity<Alojamiento>().HasKey(a => a.AlojamientoId);
        modelBuilder.Entity<ExperienciaGastronomica>().HasKey(e => e.GastronomiaId);
        modelBuilder.Entity<Restaurante>().HasKey(r => r.RestauranteId);
        modelBuilder.Entity<Recomendacion>().HasKey(r => r.RecomendacionId);
        modelBuilder.Entity<AgenciaTurismo>().HasKey(a => a.AgenciaId);
        modelBuilder.Entity<PublicacionImagen>().HasKey(pi => pi.ImagenId);
        modelBuilder.Entity<Reporte>().HasKey(r => r.ReporteId);
        modelBuilder.Entity<NotificacionAdmin>().HasKey(n => n.NotificacionId);
        modelBuilder.Entity<InvitacionAdmin>().HasKey(i => i.InvitacionId);
        modelBuilder.Entity<CorreoBloqueado>().HasKey(c => c.CorreoBloqueadoId);
        // Un usuario guarda cada publicación una sola vez
        modelBuilder.Entity<PublicacionFavorita>().HasKey(f => new { f.UsuarioId, f.PublicacionId });

        // --- ÍNDICES / RESTRICCIONES ---
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Usuario>().Property(u => u.InstagramUsuario).HasMaxLength(30);
        modelBuilder.Entity<Pais>().HasIndex(p => p.CodigoIso).IsUnique();
        modelBuilder.Entity<Ciudad>().HasIndex(c => new { c.PaisId, c.Nombre }).IsUnique();
        modelBuilder.Entity<TokenRecuperacion>().HasIndex(t => t.Token);
        modelBuilder.Entity<Publicacion>().HasIndex(p => p.Eliminada);
        modelBuilder.Entity<InvitacionAdmin>().HasIndex(i => i.TokenHash).IsUnique();
        modelBuilder.Entity<InvitacionAdmin>().HasIndex(i => i.Email);
        modelBuilder.Entity<InvitacionAdmin>().Property(i => i.Email).HasMaxLength(200);
        modelBuilder.Entity<InvitacionAdmin>().Property(i => i.TokenHash).HasMaxLength(64);
        modelBuilder.Entity<CorreoBloqueado>().HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<CorreoBloqueado>().Property(c => c.Email).HasMaxLength(200);
        modelBuilder.Entity<CorreoBloqueado>().Property(c => c.Motivo).HasMaxLength(300);

        // PRECISIÓN DE COORDENADAS (NUMERIC(9,6))
        modelBuilder.Entity<Alojamiento>()
            .Property(a => a.Latitud).HasPrecision(9, 6);
        modelBuilder.Entity<Alojamiento>()
            .Property(a => a.Longitud).HasPrecision(9, 6);

        modelBuilder.Entity<Restaurante>()
            .Property(r => r.Latitud).HasPrecision(9, 6);
        modelBuilder.Entity<Restaurante>()
            .Property(r => r.Longitud).HasPrecision(9, 6);

        modelBuilder.Entity<AgenciaTurismo>()
            .Property(a => a.Latitud).HasPrecision(9, 6);
        modelBuilder.Entity<AgenciaTurismo>()
            .Property(a => a.Longitud).HasPrecision(9, 6);

        // --- RELACIONES 1 A 1 ---
        modelBuilder.Entity<Publicacion>()
            .HasOne(p => p.Alojamiento)
            .WithOne(a => a.Publicacion)
            .HasForeignKey<Alojamiento>(a => a.PublicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Publicacion>()
            .HasOne(p => p.ExperienciaGastronomica)
            .WithOne(eg => eg.Publicacion)
            .HasForeignKey<ExperienciaGastronomica>(eg => eg.PublicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Publicacion>()
            .HasOne(p => p.Recomendacion)
            .WithOne(r => r.Publicacion)
            .HasForeignKey<Recomendacion>(r => r.PublicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- RELACIONES 1 A N ---
        modelBuilder.Entity<Ciudad>()
            .HasOne(c => c.Pais)
            .WithMany(p => p.Ciudades)
            .HasForeignKey(c => c.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Publicacion>()
            .HasOne(p => p.Ciudad)
            .WithMany(c => c.Publicaciones)
            .HasForeignKey(p => p.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Publicacion>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Publicaciones)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PublicacionImagen>()
            .HasOne(pi => pi.Publicacion)
            .WithMany(p => p.Imagenes)
            .HasForeignKey(pi => pi.PublicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Restaurante>()
            .HasOne(r => r.ExperienciaGastronomica)
            .WithMany(eg => eg.Restaurantes)
            .HasForeignKey(r => r.GastronomiaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AgenciaTurismo>()
            .HasOne(a => a.Recomendacion)
            .WithMany(r => r.Agencias)
            .HasForeignKey(a => a.RecomendacionId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- REPORTES ---
        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.UsuarioReportador)
            .WithMany(u => u.ReportesCreados)
            .HasForeignKey(r => r.UsuarioReportadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Publicacion)
            .WithMany(p => p.Reportes)
            .HasForeignKey(r => r.PublicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- FAVORITOS ---
        modelBuilder.Entity<PublicacionFavorita>()
            .HasOne(f => f.Usuario)
            .WithMany(u => u.Favoritos)
            .HasForeignKey(f => f.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PublicacionFavorita>()
            .HasOne(f => f.Publicacion)
            .WithMany()
            .HasForeignKey(f => f.PublicacionId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- INVITACIONES ADMIN ---
        modelBuilder.Entity<InvitacionAdmin>()
            .HasOne(i => i.CreadaPor)
            .WithMany()
            .HasForeignKey(i => i.CreadaPorUsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- DATOS INICIALES ---
        modelBuilder.Entity<Pais>().HasData(PaisesSeed.Obtener());
    }
}
