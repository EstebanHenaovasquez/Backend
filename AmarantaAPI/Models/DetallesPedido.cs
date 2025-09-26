using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class DetallesPedido
{
    public int CodigoDetallePedido { get; set; }

    public int? CodigoPedido { get; set; }

    public int? CodigoProducto { get; set; }

    public int? Cantidad { get; set; }

    public double? Subtotal { get; set; }

    public virtual Pedido? CodigoPedidoNavigation { get; set; }

    public virtual Producto? CodigoProductoNavigation { get; set; }
}
