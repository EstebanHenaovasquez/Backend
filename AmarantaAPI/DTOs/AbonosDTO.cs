using System.ComponentModel.DataAnnotations;

namespace AmarantaAPI.DTOs
{
    public class CrearAbonoDTO
    {
        [Required]
        public string FechaAbono { get; set; }

        [Required]
        public double Abonado { get; set; }

        [Required]
        public int IdUsuario { get; set; }
    }

    public class ActualizarAbonoDTO
    {
        public string? FechaAbono { get; set; }
        public double? Abonado { get; set; }
        public int? IdUsuario { get; set; }
    }
}
