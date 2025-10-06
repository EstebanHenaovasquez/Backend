namespace AmarantaAPI.DTOs
{
    public class CrearProveedorDTO
    {
        public string Nit { get; set; }
        public string NombreEmpresa { get; set; }
        public string Representante { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }

    public class ActualizarProveedorDTO
    {
        public string? Nit { get; set; }
        public string? NombreEmpresa { get; set; }
        public string? Representante { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
    }

    public class ProveedorDTO
    {
        public int IdProveedor { get; set; }
        public string Nit { get; set; }
        public string NombreEmpresa { get; set; }
        public string Representante { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
    }
}

