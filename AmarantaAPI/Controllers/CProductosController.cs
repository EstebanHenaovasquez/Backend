using AmarantaAPI.DTOs;
using AmarantaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmarantaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CProductosController : Controller
    {
        private readonly AmarantaFinalContext _context;

        public CProductosController(AmarantaFinalContext context)
        {
            _context = context;
        }

        // GET: api/CProductos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CProducto>>> GetCProductos()
        {
            return await _context.CProductos.ToListAsync();
        }

        // GET: api/CProductos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CProducto>> GetCProducto(int id)
        {
            var cProducto = await _context.CProductos.FindAsync(id);

            if (cProducto == null)
            {
                return NotFound();
            }

            return cProducto;
        }

        // PUT: api/CProductos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCProducto(int id, CProducto cProducto)
        //{
        //    if (id != cProducto.IdCategoria)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(cProducto).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CProductoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, [FromBody] ActualizarCProductoDTO dto)
        {
            var categoria = await _context.CProductos.FindAsync(id);
            if (categoria == null)
                return NotFound();

            if (dto.NombreCategoria != null) categoria.NombreCategoria = dto.NombreCategoria;
            if (dto.Descripcion != null) categoria.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            // ✅ devolvemos el objeto actualizado
            return Ok(categoria);
        }


        [HttpPost]        
        public async Task<ActionResult<CProducto>> PostCProducto([FromBody] CrearCProductoDTO dto)
        {
            var nuevaCategoria = new CProducto
            {
                NombreCategoria = dto.NombreCategoria,
                Descripcion = dto.Descripcion
            };

            _context.CProductos.Add(nuevaCategoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCProducto", new { id = nuevaCategoria.IdCategoria }, nuevaCategoria);
        }

        // DELETE: api/CProductos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCProducto(int id)
        {
            var cProducto = await _context.CProductos.FindAsync(id);
            if (cProducto == null)
            {
                return NotFound();
            }

            _context.CProductos.Remove(cProducto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CProductoExists(int id)
        {
            return _context.CProductos.Any(e => e.IdCategoria == id);
        }
    }
}
