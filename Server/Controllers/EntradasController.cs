using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Parcial2_Kissland.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntradasController : ControllerBase
    {
        private readonly Context _context;

        public EntradasController(Context context)
        {
            _context = context;
        }

        // GET: api/Entradas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entradas>>> GetEntradas()
        {
            if (_context.entradas == null)
            {
                return NotFound();
            }
            return await _context.entradas.ToListAsync();
        }

        // GET: api/Entradas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entradas>> GetEntradas(int id)
        {
            if (_context.entradas == null)
            {
                return NotFound();
            }
            var entradas = await _context.entradas
            .Include(c => c.EntradasDetalles)
            .Where(c => c.EntradaId == id)
            .FirstOrDefaultAsync();

            if (entradas == null)
            {
                return NotFound();
            }

            return entradas;
        }

        // POST: api/Entradas
        [HttpPost]
        public async Task<ActionResult<Entradas>> PostEntradas(Entradas entradas)
        {
            if (!EntradasExists(entradas.EntradaId))
                _context.entradas.Add(entradas);
            else
                _context.entradas.Update(entradas);

            await _context.SaveChangesAsync();
            return Ok(entradas);
        }

        // DELETE: api/Entradas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntradas(int id)
        {
            if (_context.entradas == null)
            {
                return NotFound();
            }
            var entradas = await _context.entradas.FindAsync(id);
            if (entradas == null)
            {
                return NotFound();
            }

            _context.entradas.Remove(entradas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntradasExists(int id)
        {
            return (_context.entradas?.Any(e => e.EntradaId == id)).GetValueOrDefault();
        }
    }
}