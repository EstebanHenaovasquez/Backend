using System.ComponentModel.DataAnnotations;

namespace AmarantaAPI.DTOs
{
    public class CrearProductoDTO
    {
        [Required]
        public string NombreProducto { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public double Precio { get; set; }

        [Required]
        public int IdCategoria { get; set; }

        public IFormFile Imagen { get; set; }
    }

    public class ActualizarProductoDTO
    {
        public string? NombreProducto { get; set; }
        public string? Imagen { get; set; }
        public int? Stock { get; set; }
        public double? Precio { get; set; }
        public int? IdCategoria { get; set; }
        public bool? Estado { get; set; }
    }
}
