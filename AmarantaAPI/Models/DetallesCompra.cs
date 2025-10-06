using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class DetallesCompra
{
    public int CodigoDetalleCompra { get; set; }

    

    public int? CodigoCompra { get; set; }

    public int? CodigoProducto { get; set; }

    public int? Cantidad { get; set; }

    public string? NombreProducto { get; set; }

    public double? PrecioUnitario { get; set; }

    public double? Subtotal { get; set; }

    public virtual Compra? CodigoCompraNavigation { get; set; }

    public virtual Producto? CodigoProductoNavigation { get; set; }
    
}
