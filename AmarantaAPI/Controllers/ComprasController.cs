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
    public class ComprasController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;

        public ComprasController(AmarantaFinalContext context)
        {
            _context = context;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraResponseDTO>>> GetCompras()
        {
            return await _context.Compras
                .Include(c => c.IdProveedorNavigation) 
                .Include(c => c.IdUsuarioNavigation)
                .Select(c => new CompraResponseDTO
                {
                    CodigoCompra = c.CodigoCompra,
                    FechaCompra = c.FechaCompra,
                    PrecioTotal = c.PrecioTotal,
                    Estado = c.Estado,
                    IdUsuario = c.IdUsuario,
                    NombreUsuario = c.IdUsuarioNavigation != null
                    ? c.IdUsuarioNavigation.Nombre + " " + c.IdUsuarioNavigation.Apellido
                    : null,
                    IdProveedor = c.IdProveedor,
                    NombreEmpresa = c.IdProveedorNavigation != null
                    ? c.IdProveedorNavigation.NombreEmpresa
                    : null
                })
                .ToListAsync();
        }

        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .FirstOrDefaultAsync(c => c.CodigoCompra == id);

            if (compra == null)
            {
                return NotFound();
            }

            return compra;
        }

        // PUT: api/Compras/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCompra(int id, [FromBody] ActualizarCompraDTO dto)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null) return NotFound();

            if (dto.FechaCompra != null) compra.FechaCompra = dto.FechaCompra;
            if (dto.PrecioTotal.HasValue) compra.PrecioTotal = dto.PrecioTotal;
            if (dto.Estado != null) compra.Estado = dto.Estado;
            if (dto.IdUsuario.HasValue) compra.IdUsuario = dto.IdUsuario;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // POST: api/Compras
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Compra>> CrearCompra([FromBody] CrearCompraDTO dto)
        {
            var compra = new Compra
            {
                FechaCompra = dto.FechaCompra,
                PrecioTotal = dto.PrecioTotal,
                Estado = dto.Estado ?? "Pendiente", // valor por defecto si no se envía
                IdUsuario = dto.IdUsuario,
                IdProveedor = dto.IdProveedor 
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompra), new { id = compra.CodigoCompra }, compra);
        }


        // DELETE: api/Compras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.CodigoCompra == id);
        }
    }
}
