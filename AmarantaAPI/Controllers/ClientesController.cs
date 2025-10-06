using AmarantaAPI.DTOs;
using AmarantaAPI.Models;
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

        public ClientesController(AmarantaFinalContext context)
        {
            _context = context;
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
        public async Task<IActionResult> EditarCliente(int id, [FromBody] ActualizarClienteDTO dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            if (dto.ImagenPerfil != null) cliente.ImagenPerfil = dto.ImagenPerfil;
            if (dto.TipoDocumento != null) cliente.TipoDocumento = dto.TipoDocumento;
            if (dto.Documento != null) cliente.Documento = dto.Documento;
            if (dto.Nombre != null) cliente.Nombre = dto.Nombre;
            if (dto.Apellido != null) cliente.Apellido = dto.Apellido;
            if (dto.Correo != null) cliente.Correo = dto.Correo;
            if (dto.Telefono != null) cliente.Telefono = dto.Telefono;
            if (dto.Clave != null) cliente.Clave = dto.Clave;
            if (dto.Departamento != null) cliente.Departamento = dto.Departamento;
            if (dto.Municipio != null) cliente.Municipio = dto.Municipio;
            if (dto.Direccion != null) cliente.Direccion = dto.Direccion;
            if (dto.IdRol.HasValue) cliente.IdRol = dto.IdRol;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente([FromBody] CrearClienteDTO dto)
        {
            var cliente = new Cliente
            {
                ImagenPerfil = dto.ImagenPerfil,
                TipoDocumento = dto.TipoDocumento,  
                Documento = dto.Documento,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Clave = dto.Clave, // Idealmente encriptar aquí
                Departamento = dto.Departamento,
                Municipio = dto.Municipio,
                Direccion = dto.Direccion,
                IdRol = dto.IdRol
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, cliente);
        }


        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

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
