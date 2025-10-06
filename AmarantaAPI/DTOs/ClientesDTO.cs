using System.ComponentModel.DataAnnotations;

namespace AmarantaAPI.DTOs
{
    public class CrearClienteDTO
    {
        public string? ImagenPerfil { get; set; }
        [Required]
        public string TipoDocumento { get; set; }

        [Required]
        public string Documento { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }

        [Required]
        public string Clave { get; set; }

        public string? Departamento { get; set; }
        public string? Municipio { get; set; }
        public string? Direccion { get; set; }

        [Required]
        public int IdRol { get; set; }
    }

    public class ActualizarClienteDTO
    {
        public string? ImagenPerfil { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Clave { get; set; }
        public string? Departamento { get; set; }
        public string? Municipio { get; set; }
        public string? Direccion { get; set; }
        public int? IdRol { get; set; }
    }
}
