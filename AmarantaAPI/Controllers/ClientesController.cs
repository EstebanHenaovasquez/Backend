using AmarantaAPI.DTOs;
using AmarantaAPI.Models;
using AmarantaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmarantaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;

        private readonly CloudinaryService _cloudinaryService;

        public ClientesController(AmarantaFinalContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromForm] ActualizarClienteDTO dto, IFormFile? nuevaImagen)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null) return NotFound();

            if (nuevaImagen != null)
                cliente.ImagenPerfil = await _cloudinaryService.SubirImagenAsync(nuevaImagen);

            if (dto.Nombre != null) cliente.Nombre = dto.Nombre;
            if (dto.Apellido != null) cliente.Apellido = dto.Apellido;
            if (dto.Correo != null) cliente.Correo = dto.Correo;
            if (dto.Telefono != null) cliente.Telefono = dto.Telefono;
            if (dto.Clave != null) cliente.Clave = dto.Clave;
            if (dto.Documento != null) cliente.Documento = dto.Documento;
            if (dto.Departamento != null) cliente.Departamento = dto.Departamento;
            if (dto.Municipio != null) cliente.Municipio = dto.Municipio;
            if (dto.Direccion != null) cliente.Direccion = dto.Direccion;

            // ✅ Sincronizar usuario
            if (cliente.Usuario != null)
            {
                cliente.Usuario.ImagenPerfil = cliente.ImagenPerfil;
                cliente.Usuario.TipoDocumento = cliente.TipoDocumento;
                cliente.Usuario.Documento = cliente.Documento;
                cliente.Usuario.Nombre = cliente.Nombre;
                cliente.Usuario.Apellido = cliente.Apellido;
                cliente.Usuario.Correo = cliente.Correo;
                cliente.Usuario.Telefono = cliente.Telefono;
                cliente.Usuario.Clave = cliente.Clave;
                cliente.Usuario.Departamento = cliente.Departamento;
                cliente.Usuario.Municipio = cliente.Municipio;
                cliente.Usuario.Direccion = cliente.Direccion;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostCliente([FromForm] CrearClienteDTO dto, IFormFile? imagen)
        {
            if (await _context.Clientes.AnyAsync(c => c.Correo == dto.Correo))
                return BadRequest("El correo ya está registrado como cliente.");

            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya está registrado como usuario.");

            // 🟢 Subir imagen (opcional)
            if (imagen != null)
                dto.ImagenPerfil = await _cloudinaryService.SubirImagenAsync(imagen);

            // 🟢 1️⃣ Crear usuario primero
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
                IdRol = 2, // rol cliente
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(); // ✅ genera IdUsuario

            // 🟢 2️⃣ Crear cliente vinculado
            var cliente = new Cliente
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
                IdUsuario = usuario.IdUsuario // vincular relación
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, new
            {
                exito = true,
                mensaje = "Cliente y usuario registrados exitosamente.",
                cliente.IdCliente,
                cliente.Nombre,
                cliente.Apellido,
                correo = cliente.Correo
            });
        }



        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null) return NotFound();

            if (cliente.Usuario != null)
                _context.Usuarios.Remove(cliente.Usuario);

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.IdCliente == id);
        }
    }
}
