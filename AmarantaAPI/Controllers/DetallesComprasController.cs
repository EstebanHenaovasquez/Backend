using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmarantaAPI.Models;

namespace AmarantaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallesComprasController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;

        public DetallesComprasController(AmarantaFinalContext context)
        {
            _context = context;
        }

        // ✅ GET: api/DetallesCompras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallesCompra>>> GetDetallesCompras()
        {
            return await _context.DetallesCompras
                .Include(d => d.CodigoProductoNavigation)
                .Include(d => d.CodigoCompraNavigation)
                .ToListAsync();
        }

        // ✅ GET: api/DetallesCompras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetallesCompra>> GetDetalleCompra(int id)
        {
            var detalle = await _context.DetallesCompras
                .Include(d => d.CodigoProductoNavigation)
                .Include(d => d.CodigoCompraNavigation)
                .FirstOrDefaultAsync(d => d.CodigoDetalleCompra == id);

            if (detalle == null)
                return NotFound();

            return detalle;
        }

        // ✅ GET: api/DetallesCompras/compra/5
        [HttpGet("compra/{idCompra}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDetallesByCompra(int idCompra)
        {
            var detalles = await _context.DetallesCompras
                .Include(d => d.CodigoProductoNavigation)
                .Where(d => d.CodigoCompra == idCompra)
                .Select(d => new
                {
                    d.CodigoDetalleCompra,
                    d.CodigoCompra,
                    d.CodigoProducto,
                    NombreProducto = d.CodigoProductoNavigation != null ? d.CodigoProductoNavigation.NombreProducto : d.NombreProducto,
                    PrecioUnitario = d.CodigoProductoNavigation != null ? d.CodigoProductoNavigation.Precio : d.PrecioUnitario,
                    d.Cantidad,
                    d.Subtotal
                })
                .ToListAsync();

            if (detalles == null || !detalles.Any())
                return NotFound(new { message = "No se encontraron detalles para esta compra." });

            return Ok(detalles);
        }

        // ✅ POST: api/DetallesCompras
        [HttpPost]
        public async Task<ActionResult<DetallesCompra>> PostDetallesCompra(DetallesCompra detallesCompra)
        {
            _context.DetallesCompras.Add(detallesCompra);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDetalleCompra), new { id = detallesCompra.CodigoDetalleCompra }, detallesCompra);
        }

        // ✅ POST: api/DetallesCompras/multiple
        // 👉 Permite registrar varios productos en una sola compra
        [HttpPost("multiple")]
        public async Task<IActionResult> PostDetallesCompraMultiple([FromBody] List<DetallesCompra> detallesCompras)
        {
            if (detallesCompras == null || !detallesCompras.Any())
                return BadRequest(new { message = "No se enviaron detalles válidos." });

            await _context.DetallesCompras.AddRangeAsync(detallesCompras);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Detalles de compra agregados correctamente." });
        }

        // ✅ PUT: api/DetallesCompras/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetallesCompra(int id, DetallesCompra detallesCompra)
        {
            if (id != detallesCompra.CodigoDetalleCompra)
                return BadRequest();

            _context.Entry(detallesCompra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetallesCompraExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/DetallesCompras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetallesCompra(int id)
        {
            var detallesCompra = await _context.DetallesCompras.FindAsync(id);
            if (detallesCompra == null)
                return NotFound();

            _context.DetallesCompras.Remove(detallesCompra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetallesCompraExists(int id)
        {
            return _context.DetallesCompras.Any(e => e.CodigoDetalleCompra == id);
        }
    }
}
