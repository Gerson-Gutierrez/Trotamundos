namespace Negocio.Entidades;

    public class Pais
    {
        public int PaisId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? CodigoIso { get; set; }

        // Navegación
        public ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
    }

