using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class Pedido
{
    public int CodigoPedido { get; set; }

    public string? FechaPedido { get; set; }

    public double? PrecioTotal { get; set; }

    public string? Estado { get; set; }

    public int? IdCliente { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }
    public string? Departamento { get; set; }
    public string? Municipio { get; set; }

    public virtual ICollection<DetallesPedido> DetallesPedidos { get; set; } = new List<DetallesPedido>();

    public virtual Cliente? IdClienteNavigation { get; set; }

}
