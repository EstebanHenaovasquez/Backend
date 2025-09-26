using System.ComponentModel.DataAnnotations;

namespace AmarantaAPI.DTOs
{
    public class CrearDetalleCompraDTO
    {
        [Required]
        public int IdProveedor { get; set; }

        [Required]
        public int CodigoCompra { get; set; }

        [Required]
        public int CodigoProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public double Subtotal { get; set; }
    }

    public class ActualizarDetalleCompraDTO
    {
        public int? IdProveedor { get; set; }
        public int? CodigoCompra { get; set; }
        public int? CodigoProducto { get; set; }
        public int? Cantidad { get; set; }
        public double? Subtotal { get; set; }
    }
}
