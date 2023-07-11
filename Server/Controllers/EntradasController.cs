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
            if (_context.Entradas == null)
            {
                return NotFound();
            }
            return await _context.Entradas.ToListAsync();
        }

        // GET: api/Entradas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entradas>> GetEntradas(int id)
        {
            if (_context.Entradas == null)
            {
                return NotFound();
            }
            var entradas = await _context.Entradas
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
            {
                Productos? productos; 
                foreach(var agotado in entradas.EntradasDetalles)
                {
                    productos = _context.Productos.Find(agotado.ProductoId);
                    productos.Existencia -= (double)agotado.CantidadUtilizada;
                    _context.Entry(productos).State = EntityState.Modified;
                    _context.Entry(agotado).State = EntityState.Added;

                }
                _context.Entradas.Add(entradas);
            }
            else
            {
                var entradaAnterior = _context.Entradas
                    .Include(e => e.EntradasDetalles)
                    .AsNoTracking()
                    .FirstOrDefault(e => e.EntradaId == entradas.EntradaId);
                Productos? productos;
                foreach (var agotado in entradaAnterior.EntradasDetalles)
                {
                    productos = _context.Productos.Find(agotado.ProductoId);
                    productos.Existencia += (double)agotado.CantidadUtilizada;
                    _context.Entry(productos).State = EntityState.Modified;
                }
                productos = _context.Productos.Find(entradaAnterior.ProductoId);
                productos.Existencia -= entradaAnterior.CantidadProducida;
                _context.Entry(productos).State = EntityState.Modified;
                _context.Database.ExecuteSqlRaw($"Delete from EntradasDetalles where EntradaId = {entradas.EntradaId}");
                foreach (var agotado in entradas.EntradasDetalles)
                {
                    productos = _context.Productos.Find(agotado.ProductoId);
                    productos.Existencia -= (double)agotado.CantidadUtilizada;
                    _context.Entry(productos).State = EntityState.Modified;
                    _context.Entry(agotado).State = EntityState.Added;
                }
                productos = _context.Productos.Find(entradas.ProductoId);
                productos.Existencia += entradas.CantidadProducida;
                _context.Entry(productos).State = EntityState.Modified;
                _context.Entradas.Update(entradas);
            }
            _context.Entry(entradas).State = EntityState.Detached;
            await _context.SaveChangesAsync();
            return Ok(entradas);           
        }

        // DELETE: api/Entradas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntradas(int id)
        {
            if(_context.Entradas == null)
            {
                return NotFound();
            }
            var entradas = await _context.Entradas.FindAsync(id);
            if (entradas == null)
            {
                return NotFound();
            }
            Productos? productos;
            foreach (var agotado in entradas.EntradasDetalles)
            {
                productos = _context.Productos.Find(agotado.ProductoId);
                productos.Existencia += (double)agotado.CantidadUtilizada;
                _context.Entry(productos).State = EntityState.Modified;
            }
            productos = _context.Productos.Find(entradas.ProductoId);
            productos.Existencia -= entradas.CantidadProducida;
            _context.Entry(productos).State = EntityState.Modified;
            _context.Database.ExecuteSqlRaw($"Delete from EntradasDetalles where EntradaId = {entradas.EntradaId}");
            _context.Entradas.Remove(entradas);
            await _context.SaveChangesAsync();
            _context.Entry(entradas).State = EntityState.Detached;

            return NoContent();
        }

        private bool EntradasExists(int id)
        {
            return (_context.Entradas?.Any(e => e.EntradaId == id)).GetValueOrDefault();
        }
    }
}