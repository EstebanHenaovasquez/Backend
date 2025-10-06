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
        public int IdUsuario { get; set; }
        [Required]
        public int IdProveedor { get; set; }
    }

    public class ActualizarCompraDTO
    {
        public string? FechaCompra { get; set; }
        public double? PrecioTotal { get; set; }
        public string? Estado { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdProveedor { get; set; }
    }

    public class CompraResponseDTO
    {
        public int CodigoCompra { get; set; }
        public string? FechaCompra { get; set; }
        public double? PrecioTotal { get; set; }
        public string? Estado { get; set; }

        // Relación con usuario
        public int? IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }

        // Relación con proveedor
        public int? IdProveedor { get; set; }
        public string? NombreEmpresa { get; set; }
    }
}
