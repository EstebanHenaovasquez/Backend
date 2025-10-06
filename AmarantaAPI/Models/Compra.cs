using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmarantaAPI.Models;

public partial class Compra
{
    public int CodigoCompra { get; set; }

    [Column("Fecha_Compra")]
    public string? FechaCompra { get; set; }

    [Column("Precio_Total")]
    public double? PrecioTotal { get; set; }

    public string? Estado { get; set; }

    [Column("Id_Usuario")]
    public int? IdUsuario { get; set; }

    [Column("Id_Proveedor")]
    public int? IdProveedor { get; set; }

    public virtual Proveedore? IdProveedorNavigation { get; set; }

    public virtual ICollection<DetallesCompra> DetallesCompras { get; set; } = new List<DetallesCompra>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
