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
    public class PedidoesController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;

        public PedidoesController(AmarantaFinalContext context)
        {
            _context = context;
        }

        // GET: api/Pedidoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return await _context.Pedidos.ToListAsync();
        }

        // GET: api/Pedidoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return pedido;
        }

        // PUT: api/Pedidoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedido)
        {
            if (id != pedido.CodigoPedido)
            {
                return BadRequest();
            }

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Pedidoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("crear-con-detalles")]
        public async Task<ActionResult> CrearPedidoConDetalles([FromBody] PedidoConDetallesDTO dto)
        {
            var pedido = new Pedido
            {
                FechaPedido = dto.FechaPedido,
                IdCliente = dto.IdCliente,
                IdUsuario = dto.IdUsuario,
                Estado = "Pendiente"
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync(); // Genera CodigoPedido

            double total = 0;

            foreach (var detalleDto in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalleDto.CodigoProducto);
                if (producto == null)
                    return BadRequest($"Producto {detalleDto.CodigoProducto} no encontrado.");

                var precio = producto.Precio ?? 0;
                var subtotal = precio * detalleDto.Cantidad;

                var detalle = new DetallesPedido
                {
                    CodigoPedido = pedido.CodigoPedido,
                    CodigoProducto = detalleDto.CodigoProducto,
                    Cantidad = detalleDto.Cantidad,
                    Subtotal = subtotal
                };

                total += subtotal;
                _context.DetallesPedidos.Add(detalle);
            }

            pedido.PrecioTotal = total;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.CodigoPedido }, new { pedido.CodigoPedido, total });
        }


        // DELETE: api/Pedidoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.CodigoPedido == id);
        }
    }
}
