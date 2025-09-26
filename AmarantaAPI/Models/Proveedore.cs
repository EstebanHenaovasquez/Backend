using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class Proveedore
{
    public int IdProveedor { get; set; }

    public string? Nit { get; set; }

    public string? NombreEmpresa { get; set; }

    public string? Representante { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<DetallesCompra> DetallesCompras { get; set; } = new List<DetallesCompra>();
}
