using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public CargoController(DBVETPETContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cargo>>> GetCargos()
        {
            try
            {
                var cargos = await _context.Cargos.ToListAsync();
                return Ok(cargos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener los cargos: {ex.Message}");
            }
        }

        // GET: api/Cargo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cargo>> GetCargo(int id)
        {
            try
            {
                var cargo = await _context.Cargos.FindAsync(id);

                if (cargo == null)
                {
                    return NotFound();
                }

                return Ok(cargo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener el cargo: {ex.Message}");
            }
        }

        // POST: api/Cargo
        [HttpPost]
        public async Task<IActionResult> PostCargo(Cargo cargo)
        {
            try
            {
                _context.Cargos.Add(cargo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCargo), new { id = cargo.CargoId }, cargo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al crear el cargo: {ex.Message}");
            }
        }

        // PUT: api/Cargo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCargo(int id, Cargo cargo)
        {
            try
            {
                if (id != cargo.CargoId)
                {
                    return BadRequest("El ID del cargo no coincide con el cuerpo de la solicitud.");
                }

                _context.Entry(cargo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CargoExists(id))
                {
                    return NotFound("El cargo no existe.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al actualizar el cargo: {ex.Message}");
            }
        }

        // DELETE: api/Cargo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCargo(int id)
        {
            try
            {
                var cargo = await _context.Cargos.FindAsync(id);
                if (cargo == null)
                {
                    return NotFound("El cargo no existe.");
                }

                _context.Cargos.Remove(cargo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el cargo: {ex.Message}");
            }
        }

        private bool CargoExists(int id)
        {
            return _context.Cargos.Any(e => e.CargoId == id);
        }
    }
}