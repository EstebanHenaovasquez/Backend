using AmarantaAPI.DTOs;
using AmarantaAPI.Helpers;
using AmarantaAPI.Models;
using AmarantaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                return Conflict(new { campo = "correo", mensaje = "El correo ya está registrado." });

            if (await _context.Usuarios.AnyAsync(u => u.Documento == dto.Documento))
                return Conflict(new { campo = "documento", mensaje = "El documento ya está registrado." });

            if (imagen != null)
                dto.ImagenPerfil = await _cloudinaryService.SubirImagenAsync(imagen);

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

            // ✅ Crear cliente sincronizado
            var cliente = new Cliente
            {
                ImagenPerfil = usuario.ImagenPerfil,
                TipoDocumento = usuario.TipoDocumento,
                Documento = usuario.Documento,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                Clave = usuario.Clave,
                Departamento = usuario.Departamento,
                Municipio = usuario.Municipio,
                Direccion = usuario.Direccion,
                IdRol = 2,
                IdUsuario = usuario.IdUsuario
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var enviado = await _emailHelper.EnviarCorreoAsync(
                usuario.Correo,
                "Código de verificación de Amaranta",
                codigo,
                "Tu código de verificación es el siguiente:"
            );

            if (!enviado)
                return StatusCode(500, new { mensaje = "Usuario creado, pero error al enviar el correo." });

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, new
            {
                exito = true,
                mensaje = "Usuario y cliente registrados exitosamente.",
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Apellido,
                idRol = usuario.IdRol,
                rol = usuario.IdRolNavigation?.NombreRol,
                correo = usuario.Correo
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromForm] ActualizarUsuarioDTO dto, IFormFile? nuevaImagen)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null) return NotFound();

            if (nuevaImagen != null)
                usuario.ImagenPerfil = await _cloudinaryService.SubirImagenAsync(nuevaImagen);

            if (dto.Nombre != null) usuario.Nombre = dto.Nombre;
            if (dto.Apellido != null) usuario.Apellido = dto.Apellido;
            if (dto.Correo != null) usuario.Correo = dto.Correo;
            if (dto.Telefono != null) usuario.Telefono = dto.Telefono;
            if (dto.Clave != null) usuario.Clave = dto.Clave;
            if (dto.Documento != null) usuario.Documento = dto.Documento;
            if (dto.Departamento != null) usuario.Departamento = dto.Departamento;
            if (dto.Municipio != null) usuario.Municipio = dto.Municipio;
            if (dto.Direccion != null) usuario.Direccion = dto.Direccion;

            // ✅ Sincronizar con Cliente
            if (usuario.Cliente != null)
            {
                usuario.Cliente.ImagenPerfil = usuario.ImagenPerfil;
                usuario.Cliente.TipoDocumento = usuario.TipoDocumento;
                usuario.Cliente.Documento = usuario.Documento;
                usuario.Cliente.Nombre = usuario.Nombre;
                usuario.Cliente.Apellido = usuario.Apellido;
                usuario.Cliente.Correo = usuario.Correo;
                usuario.Cliente.Telefono = usuario.Telefono;
                usuario.Cliente.Clave = usuario.Clave;
                usuario.Cliente.Departamento = usuario.Departamento;
                usuario.Cliente.Municipio = usuario.Municipio;
                usuario.Cliente.Direccion = usuario.Direccion;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null) return NotFound();

            if (usuario.Cliente != null)
                _context.Clientes.Remove(usuario.Cliente);

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //[HttpPost("Login")]
        //public async Task<IActionResult> Login([FromBody] LoginDTO login)
        //{
        //    if (login == null || string.IsNullOrWhiteSpace(login.Correo) || string.IsNullOrWhiteSpace(login.Clave))
        //    {
        //        return BadRequest(new { exito = false, mensaje = "Datos incompletos." });
        //    }

        //    var usuario = await _context.Usuarios
        //        .Include(u => u.IdRolNavigation)
        //        .Include(u => u.Cliente) // 👈 Por si el usuario es cliente
        //        .FirstOrDefaultAsync(u => u.Correo == login.Correo && u.Clave == login.Clave);

        //    if (usuario == null)
        //    {
        //        return Unauthorized(new { exito = false, mensaje = "Correo o clave incorrectos." });
        //    }

        //    // Generar y guardar código
        //    var codigo = new Random().Next(100000, 999999).ToString();
        //    usuario.CodigoVerificacion = codigo;
        //    await _context.SaveChangesAsync();

        //    // Enviar correo
        //    var enviado = await _emailHelper.EnviarCorreoAsync(
        //        usuario.Correo,
        //        "Código de verificación de Amaranta",
        //        codigo,
        //        "Tu código de verificación es el siguiente:"
        //    );

        //    if (!enviado)
        //    {
        //        return StatusCode(500, new { exito = false, mensaje = "No se pudo enviar el código al correo." });
        //    }

        //    // ✅ Respuesta completa
        //    return Ok(new
        //    {
        //        exito = true,
        //        mensaje = "Código enviado al correo.",
        //        usuario = new
        //        {
        //            idUsuario = usuario.IdUsuario,
        //            nombre = usuario.Nombre,
        //            apellido = usuario.Apellido,
        //            correo = usuario.Correo,
        //            rol = usuario.IdRolNavigation?.NombreRol,
        //            idCliente = usuario.Cliente?.IdCliente // 👈 si aplica
        //        }
        //    });
        //}

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Correo) || string.IsNullOrWhiteSpace(login.Clave))
            {
                return BadRequest(new { exito = false, mensaje = "Datos incompletos." });
            }

            try
            {
                // 🔥 MEJORA: Incluir explícitamente la relación con Cliente
                var usuario = await _context.Usuarios
                    .Include(u => u.IdRolNavigation)
                    .Include(u => u.Cliente) // 👈 ESTA LÍNEA ES CRÍTICA
                    .FirstOrDefaultAsync(u => u.Correo == login.Correo && u.Clave == login.Clave);

                if (usuario == null)
                {
                    return Unauthorized(new { exito = false, mensaje = "Correo o clave incorrectos." });
                }

                // Generar código
                var codigo = new Random().Next(100000, 999999).ToString();
                usuario.CodigoVerificacion = codigo;
                await _context.SaveChangesAsync();

                // Enviar correo
                var enviado = await _emailHelper.EnviarCorreoAsync(
                    usuario.Correo,
                    "Código de verificación de Amaranta",
                    codigo,
                    "Tu código de verificación es el siguiente:"
                );

                if (!enviado)
                {
                    return StatusCode(500, new { exito = false, mensaje = "No se pudo enviar el código al correo." });
                }

                // 🔥 VERIFICAR Y OBTENER idCliente
                int? idCliente = null;

                // Si el usuario tiene un cliente asociado
                if (usuario.Cliente != null)
                {
                    idCliente = usuario.Cliente.IdCliente;
                }
                else
                {
                    // Si no tiene cliente, buscar o crear uno
                    var clienteExistente = await _context.Clientes
                        .FirstOrDefaultAsync(c => c.IdUsuario == usuario.IdUsuario);

                    if (clienteExistente != null)
                    {
                        idCliente = clienteExistente.IdCliente;
                    }
                    else
                    {
                        // Crear cliente automáticamente si no existe
                        var nuevoCliente = new Cliente
                        {
                            ImagenPerfil = usuario.ImagenPerfil,
                            TipoDocumento = usuario.TipoDocumento,
                            Documento = usuario.Documento,
                            Nombre = usuario.Nombre,
                            Apellido = usuario.Apellido,
                            Correo = usuario.Correo,
                            Telefono = usuario.Telefono,
                            Clave = usuario.Clave,
                            Departamento = usuario.Departamento,
                            Municipio = usuario.Municipio,
                            Direccion = usuario.Direccion,
                            IdRol = usuario.IdRol,
                            IdUsuario = usuario.IdUsuario
                        };

                        _context.Clientes.Add(nuevoCliente);
                        await _context.SaveChangesAsync();
                        idCliente = nuevoCliente.IdCliente;
                    }
                }

                // ✅ Respuesta completa CON idCliente
                return Ok(new
                {
                    exito = true,
                    mensaje = "Código enviado al correo.",
                    usuario = new
                    {
                        idUsuario = usuario.IdUsuario,
                        nombre = usuario.Nombre,
                        apellido = usuario.Apellido,
                        correo = usuario.Correo,
                        rol = usuario.IdRolNavigation?.NombreRol,
                        idCliente = idCliente // 👈 AHORA SIEMPRE TENDRÁ VALOR
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error interno del servidor." });
            }
        }


        public class CorreoDTO
        {
            public string Correo { get; set; }
        }

        [HttpPost("EnviarCodigoRegistro")]
        public async Task<IActionResult> EnviarCodigoRegistro([FromBody] CorreoDTO dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo);
            if (usuario == null)
                return NotFound(new { mensaje = "Correo no registrado." });

            var codigo = new Random().Next(100000, 999999).ToString();
            usuario.CodigoVerificacion = codigo;
            await _context.SaveChangesAsync();


            var enviado = await _emailHelper.EnviarCorreoAsync(
                dto.Correo,
                "Código de verificación de Amaranta",
                codigo,
                "Tu código de verificación es el siguiente:"
            );

            if (!enviado)
                return StatusCode(500, new { mensaje = "Error al enviar el correo." });

            return Ok(new { mensaje = "Código enviado correctamente." });
        }

        //[HttpPost("VerificarCodigo")]
        //public async Task<IActionResult> VerificarCodigo([FromBody] CodigoVerificacionDTO dto)
        //{
        //    if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Codigo))
        //        return BadRequest(new { exito = false, mensaje = "Correo o código inválido." });

        //    var usuario = await _context.Usuarios
        //        .Include(u => u.IdRolNavigation)
        //        .FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.CodigoVerificacion == dto.Codigo);

        //    if (usuario == null)
        //        return Unauthorized(new { exito = false, mensaje = "Código incorrecto o ya expirado." });

        //    // ✅ Limpia el código para no reutilizarlo
        //    usuario.CodigoVerificacion = null;
        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        exito = true,
        //        mensaje = "Código verificado correctamente.",
        //        usuario = new
        //        {
        //            nombre = usuario.Nombre,
        //            apellido = usuario.Apellido,
        //            correo = usuario.Correo,
        //            rol = usuario.IdRolNavigation?.NombreRol,
        //            idCliente = usuario.Cliente?.IdCliente
        //        }
        //    });
        //}

        [HttpPost("VerificarCodigo")]
        public async Task<IActionResult> VerificarCodigo([FromBody] CodigoVerificacionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(new { exito = false, mensaje = "Correo o código inválido." });

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.Cliente) // 👈 INCLUIR CLIENTE
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.CodigoVerificacion == dto.Codigo);

            if (usuario == null)
                return Unauthorized(new { exito = false, mensaje = "Código incorrecto o ya expirado." });

            // 🔥 Asegurar que existe el cliente
            if (usuario.Cliente == null)
            {
                // Crear cliente si no existe
                var nuevoCliente = new Cliente
                {
                    ImagenPerfil = usuario.ImagenPerfil,
                    TipoDocumento = usuario.TipoDocumento,
                    Documento = usuario.Documento,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    Clave = usuario.Clave,
                    Departamento = usuario.Departamento,
                    Municipio = usuario.Municipio,
                    Direccion = usuario.Direccion,
                    IdRol = usuario.IdRol,
                    IdUsuario = usuario.IdUsuario
                };

                _context.Clientes.Add(nuevoCliente);
                await _context.SaveChangesAsync();
                usuario.Cliente = nuevoCliente;
            }

            // Limpiar código
            usuario.CodigoVerificacion = null;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                exito = true,
                mensaje = "Código verificado correctamente.",
                usuario = new
                {
                    idUsuario = usuario.IdUsuario,
                    nombre = usuario.Nombre,
                    apellido = usuario.Apellido,
                    correo = usuario.Correo,
                    rol = usuario.IdRolNavigation?.NombreRol,
                    idCliente = usuario.Cliente.IdCliente // 👈 SIEMPRE TENDRÁ VALOR
                }
            });
        }


        [HttpGet("ObtenerPorCorreo")]
        public async Task<IActionResult> ObtenerPorCorreo([FromQuery] string correo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            return Ok(new
            {
                usuario.IdUsuario,
                usuario.ImagenPerfil,
                usuario.Documento,
                usuario.Nombre,
                usuario.Apellido,
                usuario.Correo,
                usuario.Telefono,
                usuario.Departamento,
                usuario.Municipio,
                usuario.Direccion
                //usuario.Rol
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
            Console.WriteLine($"📩 Correo recibido: {dto?.Correo}");
            Console.WriteLine($"🔑 Código recibido: {dto?.Codigo}");
            Console.WriteLine($"🆕 Nueva clave: {dto?.NuevaClave}");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null)
            {
                Console.WriteLine("❌ Usuario no encontrado.");
                return BadRequest(new { mensaje = "Usuario no encontrado." });
            }

            Console.WriteLine($"💾 Código en BD: {usuario.CodigoVerificacion}");

            if (usuario.CodigoVerificacion != dto.Codigo)
            {
                Console.WriteLine("⚠️ Código no coincide.");
                return BadRequest(new { mensaje = "Código inválido o expirado." });
            }

            usuario.Clave = dto.NuevaClave;
            usuario.CodigoVerificacion = null;
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Contraseña restablecida correctamente.");
            return Ok(new { mensaje = "Contraseña actualizada correctamente." });
        }

        [HttpPost("CerrarSesion")]
        public IActionResult CerrarSesion()
        {
            try
            {
                // Si manejas tokens o sesiones, aquí podrías invalidarlos.
                // Pero si solo usas login simple, basta con confirmar el cierre.
                return Ok(new
                {
                    exito = true,
                    mensaje = "Sesión cerrada correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Error al cerrar sesión.",
                    detalle = ex.Message
                });
            }
        }
    }
}

    
