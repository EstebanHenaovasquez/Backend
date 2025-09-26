using System.ComponentModel.DataAnnotations;

namespace AmarantaAPI.DTOs
{
    public class CrearCompraDTO
    {
        [Required]
        public string FechaCompra { get; set; }

        [Required]
        public double PrecioTotal { get; set; }

        public string? Estado { get; set; } // Opcional o automático

        [Required]
        public int IdUsuario { get; set; }  // O se puede asignar desde el token
    }

    public class ActualizarCompraDTO
    {
        public string? FechaCompra { get; set; }
        public double? PrecioTotal { get; set; }
        public string? Estado { get; set; }
        public int? IdUsuario { get; set; }
    }
}
