using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class Producto
{
    public int CodigoProducto { get; set; }

    public string? NombreProducto { get; set; }

    public string? Imagen { get; set; }

    public int? Stock { get; set; }

    public double? Precio { get; set; }

    public int? IdCategoria { get; set; }

    public virtual ICollection<DetallesCompra> DetallesCompras { get; set; } = new List<DetallesCompra>();

    public virtual ICollection<DetallesPedido> DetallesPedidos { get; set; } = new List<DetallesPedido>();

    public virtual CProducto? IdCategoriaNavigation { get; set; }
}
