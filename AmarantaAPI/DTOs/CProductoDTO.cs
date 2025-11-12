using System.ComponentModel.DataAnnotations;

namespace AmarantaAPI.DTOs
{
    public class CrearCProductoDTO
    {
        [Required]
        public string NombreCategoria { get; set; }
        public string Descripcion { get; set; }
    }

    public class ActualizarCProductoDTO
    {
        public string NombreCategoria { get; set; }
        public string Descripcion { get; set; }
        public bool? Estado { get; set; }
    }
}
