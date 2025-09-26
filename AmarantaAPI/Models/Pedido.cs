using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class Pedido
{
    public int CodigoPedido { get; set; }

    public string? FechaPedido { get; set; }

    public double? PrecioTotal { get; set; }

    public string? Estado { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdCliente { get; set; }

    public virtual ICollection<DetallesPedido> DetallesPedidos { get; set; } = new List<DetallesPedido>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
