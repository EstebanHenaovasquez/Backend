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
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;

        public PedidosController(AmarantaFinalContext context)
        {
            _context = context;
        }

        // ✅ GET todos los pedidos con cliente y detalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoResponseDTO>>> GetPedidos()
        {
            return await _context.Pedidos
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.DetallesPedidos)
                    .ThenInclude(dp => dp.CodigoProductoNavigation)
                .Select(p => new PedidoResponseDTO
                {
                    CodigoPedido = p.CodigoPedido,
                    FechaPedido = p.FechaPedido,
                    PrecioTotal = p.PrecioTotal ?? 0,
                    Estado = p.Estado ?? "Pendiente",
                    Departamento = p.Departamento ?? "Seleccionar",
                    Municipio = p.Municipio ?? "Seleccionar",
                    Direccion = p.Direccion ?? "Digitar",
                    Correo = p.Correo ?? "Digitar",
                    IdCliente = p.IdCliente ?? 0,
                    NombreCliente = p.IdClienteNavigation != null
                        ? p.IdClienteNavigation.Nombre + " " + p.IdClienteNavigation.Apellido
                        : "",
                    Detalles = p.DetallesPedidos.Select(d => new DetallePedidoResponseDTO
                    {
                        CodigoProducto = d.CodigoProducto ?? 0,
                        NombreProducto = d.CodigoProductoNavigation != null ? d.CodigoProductoNavigation.NombreProducto : d.NombreProducto,
                        Cantidad = d.Cantidad ?? 0,
                        PrecioUnitario = d.PrecioUnitario ?? 0,
                        Subtotal = d.Subtotal ?? 0
                    }).ToList()
                }).ToListAsync();
        }

        // ✅ GET pedido específico con detalles
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoResponseDTO>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.IdClienteNavigation)
                .Include(p => p.DetallesPedidos)
                    .ThenInclude(dp => dp.CodigoProductoNavigation)
                .FirstOrDefaultAsync(p => p.CodigoPedido == id);

            if (pedido == null) return NotFound();

            return new PedidoResponseDTO
            {
                CodigoPedido = pedido.CodigoPedido,
                FechaPedido = pedido.FechaPedido,
                PrecioTotal = pedido.PrecioTotal ?? 0,
                Estado = pedido.Estado ?? "Pendiente",
                Departamento = pedido.Departamento ?? "Seleccionar",
                Municipio = pedido.Municipio ?? "Seleccionar",
                Direccion = pedido.Direccion ?? "Digitar",
                Correo = pedido.Correo ?? "Digitar",
                IdCliente = pedido.IdCliente ?? 0,
                NombreCliente = pedido.IdClienteNavigation?.Nombre + " " + pedido.IdClienteNavigation?.Apellido,
                Detalles = pedido.DetallesPedidos.Select(d => new DetallePedidoResponseDTO
                {
                    CodigoProducto = d.CodigoProducto ?? 0,
                    NombreProducto = d.CodigoProductoNavigation?.NombreProducto ?? d.NombreProducto,
                    Cantidad = d.Cantidad ?? 0,
                    PrecioUnitario = d.PrecioUnitario ?? 0,
                    Subtotal = d.Subtotal ?? 0
                }).ToList()
            };
        }

        // ✅ POST crear pedido con detalles
        [HttpPost("crear-con-detalles")]
        public async Task<ActionResult> CrearPedidoConDetalles([FromBody] PedidoConDetallesDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pedido = new Pedido
                {
                    FechaPedido = dto.FechaPedido,
                    IdCliente = dto.IdCliente,
                    Estado = "Pendiente",
                    Departamento = dto.Departamento,
                    Municipio = dto.Municipio,
                    Direccion = dto.Direccion,
                    Correo = dto.Correo

                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                double PrecioTotal = 0;

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
                        PrecioUnitario = producto.Precio ?? 0,
                        Subtotal = (producto.Precio ?? 0) * detalleDto.Cantidad,
                        NombreProducto = producto.NombreProducto
                    };

                    PrecioTotal += subtotal;
                    _context.DetallesPedidos.Add(detalle);
                }

                pedido.PrecioTotal = PrecioTotal;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { mensaje = "Pedido creado con éxito", pedido.CodigoPedido, PrecioTotal });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return BadRequest(new
                {
                    error = "No se pudo crear el pedido",
                    detalle = ex.InnerException?.Message ?? ex.Message,
                    stack = ex.StackTrace // opcional, si quieres ver más detalle
                });
            }

        }
         

        // ✅ PUT cancelar pedido
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Estado = "Cancelado";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Pedido cancelado" });
        }
    }
}
