namespace Negocio.Entidades;

    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Nacionalidad { get; set; } = string.Empty;
        public string? FotoPerfilUrl { get; set; }
        // Usuario de Instagram sin @ (opcional). Se muestra a otros viajeros para que vean su perfil.
        public string? InstagramUsuario { get; set; }
        public bool EsAdmin { get; set; } = false;
        // Rol independiente de EsAdmin: el superadministrador no usa el panel de los administradores comunes.
        public bool EsSuperAdmin { get; set; } = false;
        public bool Bloqueado { get; set; } = false;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        public ICollection<TokenRecuperacion> TokensRecuperacion { get; set; } = new List<TokenRecuperacion>();
        public ICollection<Publicacion> Publicaciones { get; set; } = new List<Publicacion>();
        public ICollection<PublicacionFavorita> Favoritos { get; set; } = new List<PublicacionFavorita>();

        // Relación con los reportes que ha creado este usuario
        public ICollection<Reporte> ReportesCreados { get; set; } = new List<Reporte>();
    }

