namespace AmarantaAPI.DTOs
{
    public class PedidoConDetallesDTO
    {
        public string FechaPedido { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public int IdCliente { get; set; }
        public List<DetallePedidoDTO> Detalles { get; set; } = new();
    }

    public class DetallePedidoDTO
    {
        public int CodigoProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class PedidoResponseDTO
    {
        public int CodigoPedido { get; set; }
        public string FechaPedido { get; set; }
        public double PrecioTotal { get; set; }
        public string Estado { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public List<DetallePedidoResponseDTO> Detalles { get; set; } = new();
    }

    public class DetallePedidoResponseDTO
    {
        public int CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double Subtotal { get; set; }
    }
}
