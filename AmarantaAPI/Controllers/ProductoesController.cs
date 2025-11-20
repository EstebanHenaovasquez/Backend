using AmarantaAPI.DTOs;
using AmarantaAPI.Models;
using AmarantaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmarantaAPI.Controllers
{
    [Route("api/Productos")]
    [ApiController]
    public class ProductoesController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public ProductoesController(AmarantaFinalContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            return producto == null ? NotFound() : producto;
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> CrearProducto([FromForm] CrearProductoDTO dto, IFormFile imagen)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            string urlImagen = null;

            if (dto.Imagen != null && dto.Imagen.Length > 0)
            {
               urlImagen = await _cloudinaryService.SubirImagenAsync(imagen);
            }

            var producto = new Producto
            {
                NombreProducto = dto.NombreProducto,
                Imagen = urlImagen,
                Stock = dto.Stock,
                Precio = dto.Precio,
                IdCategoria = dto.IdCategoria,
                Estado = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.CodigoProducto }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromForm] ActualizarProductoDTO dto, IFormFile? nuevaImagen)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            if (dto.NombreProducto != null) producto.NombreProducto = dto.NombreProducto;
            if (dto.Stock.HasValue) producto.Stock = dto.Stock.Value;
            if (dto.Precio.HasValue) producto.Precio = dto.Precio.Value;
            if (dto.IdCategoria.HasValue) producto.IdCategoria = dto.IdCategoria.Value;
            if (dto.Estado.HasValue) producto.Estado = dto.Estado.Value;

            if (nuevaImagen != null)
            {
                producto.Imagen = await _cloudinaryService.SubirImagenAsync(nuevaImagen);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
