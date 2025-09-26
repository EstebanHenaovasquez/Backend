using System.ComponentModel.DataAnnotations;

namespace AmarantaAPI.DTOs
{
    public class DetallePedidoDTO
    {
        [Required]
        public int CodigoProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }
    }

    public class PedidoConDetallesDTO
    {
        [Required]
        public string FechaPedido { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        public List<DetallePedidoDTO> Detalles { get; set; } = new();
    }
}
