using System;
using System.Collections.Generic;

namespace AmarantaAPI.Models;

public partial class Abono
{
    public int CodigoAbono { get; set; }

    public string? FechaAbono { get; set; }

    public double? Abonado { get; set; }

    public int? IdUsuario { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
