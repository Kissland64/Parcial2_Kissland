using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Parcial2_Robert.Server.Controllers
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

        public bool Existe(int EntradaId)
        {
            return (_context.Entradas?.Any(e => e.EntradaId == EntradaId)).GetValueOrDefault();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entradas>>> Obtener()
        {
            if(_context.Entradas == null)
            {
                return NotFound();
            }
            else
            {
                return await _context.Entradas.ToListAsync();
            }
        }

        [HttpGet("{EntradaId}")]
        public async Task<ActionResult<Entradas>> ObtenerEntradas(int EntradaId)
        {
            if(_context.Entradas == null)
            {
                return NotFound();
            }

            var entrada = await _context.Entradas.Include(e => e.EntradasDetalles).Where( e => e.EntradaId == EntradaId).FirstOrDefaultAsync();

            if(entrada == null)
            {
                return NotFound();
            }

            foreach(var item in entrada.EntradasDetalles)
            {
                Console.WriteLine($"{item.DetalleId}, {item.EntradaId}, {item.ProductoId}, {item.CantidadUtilizada}");
            }

            return entrada;
        }
        
        [HttpPost]
        public async Task<ActionResult<Entradas>> PostEntradas(Entradas entradas)
        {
            if(!Existe(entradas.EntradaId))
            {
                Productos? producto = new Productos();
                foreach(var productoConsumido in entradas.EntradasDetalles)
                {
                    producto = _context.Productos.Find(productoConsumido.ProductoId);

                    if(producto != null)
                    {
                        producto.Existencia -= productoConsumido.CantidadUtilizada;
                        _context.Productos.Update(producto);
                        await _context.SaveChangesAsync();
                        _context.Entry(producto).State = EntityState.Detached;
                    }
                }
                await _context.Entradas.AddAsync(entradas);
            }
            else
            {
                var entradaAnterior = _context.Entradas.Include(e => e.EntradasDetalles).AsNoTracking()
                .FirstOrDefault(e => e.EntradaId == entradas.EntradaId);

                Productos? producto = new Productos();

                if(entradaAnterior != null && entradaAnterior.EntradasDetalles != null)
                {
                    foreach(var productoConsumido in entradaAnterior.EntradasDetalles)
                    {
                        if(productoConsumido != null)
                        {
                            producto = _context.Productos.Find(productoConsumido.ProductoId);

                            if(producto != null)
                            {
                                producto.Existencia +=  productoConsumido.CantidadUtilizada;
                                _context.Productos.Update(producto);
                                await _context.SaveChangesAsync();
                                _context.Entry(producto).State = EntityState.Detached;
                            }   
                        }
                    }
                }

                if(entradaAnterior != null)
                {
                    producto = _context.Productos.Find(entradaAnterior.ProductoId);

                    if(producto != null)
                    {
                        producto.Existencia -= entradaAnterior.CantidadProducida;
                        _context.Productos.Update(producto);
                        await _context.SaveChangesAsync();
                        _context.Entry(producto).State = EntityState.Detached;
                    }
                }

                _context.Database.ExecuteSqlRaw($"Delete from entradasDetalle where EntradaId = {entradas.EntradaId}");

                foreach(var productoConsumido in entradas.EntradasDetalles)
                {
                    producto = _context.Productos.Find(productoConsumido.ProductoId);

                    if(producto != null)
                    {
                        producto.Existencia -= productoConsumido.CantidadUtilizada;
                        _context.Productos.Update(producto);
                        await _context.SaveChangesAsync();
                        _context.Entry(producto).State = EntityState.Detached;
                        _context.Entry(productoConsumido).State = EntityState.Added;
                    }
                }

                producto = _context.Productos.Find(entradas.ProductoId);

                if(producto != null)
                {
                    producto.Existencia += entradas.CantidadProducida;
                    _context.Productos.Update(producto);
                    await _context.SaveChangesAsync();
                    _context.Entry(producto).State = EntityState.Detached;
                }
                _context.Entradas.Update(entradas);
            }

            await _context.SaveChangesAsync();
            _context.Entry(entradas).State = EntityState.Detached;
            return Ok(entradas);
        }

        [HttpDelete("{EntradaId}")]
        public async Task<IActionResult> EliminarEntrada(int EntradaId)
        {
            var entrada = await _context.Entradas.Include(e => e.EntradasDetalles).FirstOrDefaultAsync(e => e.EntradaId == EntradaId);

            if (entrada == null)
            {
                return NotFound();
            }

            foreach (var detalle in entrada.EntradasDetalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);

                if (producto != null)
                {
                    producto.Existencia += detalle.CantidadUtilizada;
                    _context.Productos.Update(producto);
                }
            }

            var productoPrincipal = await _context.Productos.FindAsync(entrada.ProductoId);

            if (productoPrincipal != null)
            {
                productoPrincipal.Existencia += entrada.CantidadProducida;
                _context.Productos.Update(productoPrincipal);
            }

            _context.Entradas.Remove(entrada);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}