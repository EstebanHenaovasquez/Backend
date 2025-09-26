using System.ComponentModel.DataAnnotations;


namespace AmarantaAPI.DTOs
{
    public class CrearRolDTO
    {
        [Required]
        public string? NombreRol { get; set; }

    }
    public class ActualizarRolDTO
    {
        [Required]
        public string? NombreRol { get; set; }

    }
}
