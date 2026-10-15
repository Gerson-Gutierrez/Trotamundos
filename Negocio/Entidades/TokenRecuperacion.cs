namespace Negocio.Entidades;

 public class TokenRecuperacion
 {
        public int TokenId { get; set; }
        public int UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public Usuario Usuario { get; set; } = null!;
 }

