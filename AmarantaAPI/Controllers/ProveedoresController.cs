using AmarantaAPI.DTOs;
using AmarantaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AmarantaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;

        public ProveedoresController(AmarantaFinalContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedore>>> GetProveedores()
        {
            return await _context.Proveedores.ToListAsync();
        }

        // GET: api/Proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedore>> GetProveedore(int id)
        {
            var proveedore = await _context.Proveedores.FindAsync(id);

            if (proveedore == null)
            {
                return NotFound();
            }

            return proveedore;
        }

        // PUT: api/Proveedores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, [FromBody] ActualizarProveedorDTO dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            // Solo se actualizan los campos enviados
            if (dto.Nit != null) proveedor.Nit = dto.Nit;
            if (dto.NombreEmpresa != null) proveedor.NombreEmpresa = dto.NombreEmpresa;
            if (dto.Representante != null) proveedor.Representante = dto.Representante;
            if (dto.Correo != null) proveedor.Correo = dto.Correo;
            if (dto.Telefono != null) proveedor.Telefono = dto.Telefono;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // POST: api/Proveedores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Proveedore>> PostProveedor([FromBody] CrearProveedorDTO dto)
        {
            var nuevoProveedor = new Proveedore
            {
                Nit = dto.Nit,
                NombreEmpresa = dto.NombreEmpresa,
                Representante = dto.Representante,
                Correo = dto.Correo,
                Telefono = dto.Telefono
            };

            _context.Proveedores.Add(nuevoProveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProveedore", new { id = nuevoProveedor.IdProveedor }, nuevoProveedor);
        }

        // DELETE: api/Proveedores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedore(int id)
        {
            var proveedore = await _context.Proveedores.FindAsync(id);
            if (proveedore == null)
            {
                return NotFound();
            }

            _context.Proveedores.Remove(proveedore);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProveedoreExists(int id)
        {
            return _context.Proveedores.Any(e => e.IdProveedor == id);
        }
    }
}
