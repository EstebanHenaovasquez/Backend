using AmarantaAPI.DTOs;
using AmarantaAPI.Helpers;
using AmarantaAPI.Models;
using AmarantaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmarantaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly EmailHelper _emailHelper;

        public UsuariosController(AmarantaFinalContext context, CloudinaryService cloudinaryService, EmailHelper emailHelper)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _emailHelper = emailHelper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            return usuario == null ? NotFound() : usuario;
        }

        [HttpPost]
        public async Task<ActionResult> PostUsuario([FromForm] CrearUsuarioDTO dto, IFormFile? imagen)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya está registrado.");

            if (await _context.Usuarios.AnyAsync(u => u.Documento == dto.Documento))
                return BadRequest("El documento ya está registrado.");

            if (imagen != null)
            {
                dto.ImagenPerfil = await _cloudinaryService.SubirImagenAsync(imagen);
            }

            var codigo = new Random().Next(100000, 999999).ToString();

            var usuario = new Usuario
            {
                ImagenPerfil = dto.ImagenPerfil,
                TipoDocumento = dto.TipoDocumento,
                Documento = dto.Documento,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Clave = dto.Clave,
                Departamento = dto.Departamento,
                Municipio = dto.Municipio,
                Direccion = dto.Direccion,
                IdRol = 2,
                CodigoVerificacion = codigo
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var enviado = await _emailHelper.EnviarCorreoAsync(
                usuario.Correo,
                "Código de verificación de Amaranta",
                $"Tu código de verificación es: {codigo}"
            );

            if (!enviado)
            {
                return StatusCode(500, new { mensaje = "Usuario creado, pero error al enviar el correo." });
            }

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, new
            {
                exito = true,
                mensaje = "Usuario registrado exitosamente.",
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Apellido,
                correo = usuario.Correo
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromForm] ActualizarUsuarioDTO dto, IFormFile? nuevaImagen)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            if (dto.Nombre != null) usuario.Nombre = dto.Nombre;
            if (dto.Apellido != null) usuario.Apellido = dto.Apellido;
            if (dto.Correo != null) usuario.Correo = dto.Correo;
            if (dto.Telefono != null) usuario.Telefono = dto.Telefono;
            if (dto.Clave != null) usuario.Clave = dto.Clave;
            if (dto.Documento != null) usuario.Documento = dto.Documento;
            if (dto.Departamento != null) usuario.Departamento = dto.Departamento;
            if (dto.Municipio != null) usuario.Municipio = dto.Municipio;
            if (dto.Direccion != null) usuario.Direccion = dto.Direccion;

            if (nuevaImagen != null)
            {
                usuario.ImagenPerfil = await _cloudinaryService.SubirImagenAsync(nuevaImagen);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Correo) || string.IsNullOrWhiteSpace(login.Clave))
            {
                return BadRequest(new { exito = false, mensaje = "Datos incompletos." });
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Correo == login.Correo && u.Clave == login.Clave);

            if (usuario == null)
            {
                return Unauthorized(new { exito = false, mensaje = "Correo o clave incorrectos." });
            }

            var codigo = new Random().Next(100000, 999999).ToString();
            usuario.CodigoVerificacion = codigo;
            await _context.SaveChangesAsync();

            var enviado = await _emailHelper.EnviarCorreoAsync(
                usuario.Correo,
                "Código de verificación de Amaranta",
                $"Tu código de verificación es: {codigo}");

            if (!enviado)
            {
                return StatusCode(500, new { exito = false, mensaje = "No se pudo enviar el código al correo." });
            }

            return Ok(new
            {
                exito = true,
                mensaje = "Código enviado al correo.",
                usuario = new
                {
                    nombre = usuario.Nombre,
                    apellido = usuario.Apellido,
                    correo = usuario.Correo
                }
            });
        }

        [HttpPost("EnviarCodigoRegistro")]
        public async Task<IActionResult> EnviarCodigoRegistro([FromBody] string correo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
                return NotFound(new { mensaje = "Correo no registrado." });

            var codigo = new Random().Next(100000, 999999).ToString();
            usuario.CodigoVerificacion = codigo;
            await _context.SaveChangesAsync();

            var enviado = await _emailHelper.EnviarCorreoAsync(
                correo,
                "Código de verificación",
                $"Tu código de verificación es: {codigo}"
            );

            if (!enviado)
                return StatusCode(500, new { mensaje = "Error al enviar el correo." });

            return Ok(new { mensaje = "Código enviado correctamente." });
        }

        [HttpPost("VerificarCodigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] CodigoVerificacionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { mensaje = "Correo o código inválido." });

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.Correo == dto.Correo && u.CodigoVerificacion == dto.Codigo);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Código incorrecto o ya expirado." });

            usuario.CodigoVerificacion = null;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Código verificado correctamente." });
        }

        [HttpGet("ObtenerPorCorreo")]
        public async Task<IActionResult> ObtenerPorCorreo([FromQuery] string correo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            return Ok(new
            {
                usuario.Nombre,
                usuario.Apellido,
                usuario.Correo,
                usuario.Telefono,
                usuario.Direccion,
                usuario.Departamento,
                usuario.Municipio
            });
        }

        [HttpPut("ActualizarPorCorreo/{correo}")]
        public async Task<IActionResult> ActualizarPorCorreo(string correo, [FromForm] ActualizarUsuarioDTO dto, IFormFile? nuevaImagen)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Nombre)) usuario.Nombre = dto.Nombre;
            if (!string.IsNullOrWhiteSpace(dto.Apellido)) usuario.Apellido = dto.Apellido;
            if (!string.IsNullOrWhiteSpace(dto.Clave)) usuario.Clave = dto.Clave;
            if (!string.IsNullOrWhiteSpace(dto.Telefono)) usuario.Telefono = dto.Telefono;
            if (!string.IsNullOrWhiteSpace(dto.Direccion)) usuario.Direccion = dto.Direccion;
            if (!string.IsNullOrWhiteSpace(dto.Departamento)) usuario.Departamento = dto.Departamento;
            if (!string.IsNullOrWhiteSpace(dto.Municipio)) usuario.Municipio = dto.Municipio;

            if (nuevaImagen != null)
                usuario.ImagenPerfil = await _cloudinaryService.SubirImagenAsync(nuevaImagen);

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPost("RestablecerClave")]
        public async Task<IActionResult> RestablecerClave([FromBody] RestablecerClaveDTO dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo);
            if (usuario == null || usuario.CodigoVerificacion != dto.Codigo)
                return BadRequest(new { mensaje = "Código inválido o expirado." });

            usuario.Clave = dto.NuevaClave;
            usuario.CodigoVerificacion = null;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }



    }
}
