using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class Compra
{
    public int CodigoCompra { get; set; }

    public string? FechaCompra { get; set; }

    public double? PrecioTotal { get; set; }

    public string? Estado { get; set; }

    public int? IdUsuario { get; set; }

    public virtual ICollection<DetallesCompra> DetallesCompras { get; set; } = new List<DetallesCompra>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
