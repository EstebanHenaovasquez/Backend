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
    public class AbonoesController : ControllerBase
    {
        private readonly AmarantaFinalContext _context;

        public AbonoesController(AmarantaFinalContext context)
        {
            _context = context;
        }

        // GET: api/Abonoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Abono>>> GetAbonos()
        {
            return await _context.Abonos.ToListAsync();
        }

        // GET: api/Abonoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Abono>> GetAbono(int id)
        {
            var abono = await _context.Abonos.FindAsync(id);

            if (abono == null)
            {
                return NotFound();
            }

            return abono;
        }

        // PUT: api/Abonoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarAbono(int id, [FromBody] ActualizarAbonoDTO dto)
        {
            var abono = await _context.Abonos.FindAsync(id);
            if (abono == null)
                return NotFound();

            if (dto.FechaAbono != null) abono.FechaAbono = dto.FechaAbono;
            if (dto.Abonado.HasValue) abono.Abonado = dto.Abonado;
            if (dto.IdUsuario.HasValue) abono.IdUsuario = dto.IdUsuario;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // POST: api/Abonoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Abono>> CrearAbono([FromBody] CrearAbonoDTO dto)
        {
            var abono = new Abono
            {
                FechaAbono = dto.FechaAbono,
                Abonado = dto.Abonado,
                IdUsuario = dto.IdUsuario
            };

            _context.Abonos.Add(abono);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAbono), new { id = abono.CodigoAbono }, abono);
        }


        // DELETE: api/Abonoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbono(int id)
        {
            var abono = await _context.Abonos.FindAsync(id);
            if (abono == null)
            {
                return NotFound();
            }

            _context.Abonos.Remove(abono);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AbonoExists(int id)
        {
            return _context.Abonos.Any(e => e.CodigoAbono == id);
        }
    }
}
