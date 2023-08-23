using BackEnd.Mappers;
using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriaController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public HistoriaController(DBVETPETContext context)
        {
            _context = context;
        }

        // GET: api/Historiaclinica
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoriaMapper>>> GetHistoriaclinicas()
        {
            var historiaclinicas = await _context.Historiaclinicas.ToListAsync();
            var historiaMappers = historiaclinicas.Select(h => new HistoriaMapper
            {
                HistoriaId = h.HistoriaId,
                MascotaId = h.MascotaId,
                EmpleadoId = h.EmpleadoId,
                FechaConsulta = h.FechaConsulta,
                Sintomas = h.Sintomas,
                Diagnostico = h.Diagnostico,
                Tratamiento = h.Tratamiento
            }).ToList();
            return historiaMappers;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<HistoriaViewModel>>> GetHistoriasViewModel()
        {
            var historiasViewModel = await _context.Historiaclinicas
                .Include(h => h.Mascota) 
                .Include(h => h.Empleado) 
                .Select(h => new HistoriaViewModel
                {
                    HistoriaId = h.HistoriaId,
                    Nombre = h.Mascota.Nombre, 
                    Dni = h.Empleado.Dni, 
                    FechaConsulta = h.FechaConsulta,
                    Sintomas = h.Sintomas,
                    Diagnostico = h.Diagnostico,
                    Tratamiento = h.Tratamiento
                })
                .ToListAsync();

            return historiasViewModel;
        }

        // GET: api/Historiaclinica/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HistoriaMapper>> GetHistoriaclinica(int id)
        {
            var historiaclinica = await _context.Historiaclinicas.FindAsync(id);

            if (historiaclinica == null)
            {
                return NotFound();
            }

            var historiaMapper = new HistoriaMapper
            {
                HistoriaId = historiaclinica.HistoriaId,
                MascotaId = historiaclinica.MascotaId,
                EmpleadoId = historiaclinica.EmpleadoId,
                FechaConsulta = historiaclinica.FechaConsulta,
                Sintomas = historiaclinica.Sintomas,
                Diagnostico = historiaclinica.Diagnostico,
                Tratamiento = historiaclinica.Tratamiento
            };

            return historiaMapper;
        }

        // POST: api/Historiaclinica
        [HttpPost]
        public async Task<ActionResult<HistoriaMapper>> PostHistoriaclinica(HistoriaMapper historiaMapper)
        {
            var historiaclinica = new Historiaclinica
            {
                MascotaId = historiaMapper.MascotaId,
                EmpleadoId = historiaMapper.EmpleadoId,
                FechaConsulta = DateTime.Now.Date, // Capturar la fecha actual sin la hora.
                Sintomas = historiaMapper.Sintomas,
                Diagnostico = historiaMapper.Diagnostico,
                Tratamiento = historiaMapper.Tratamiento
            };

            _context.Historiaclinicas.Add(historiaclinica);
            await _context.SaveChangesAsync();

            historiaMapper.HistoriaId = historiaclinica.HistoriaId; // Actualizar el ID en el ViewModel.

            return CreatedAtAction(nameof(GetHistoriaclinica), new { id = historiaclinica.HistoriaId }, historiaMapper);
        }

        // PUT: api/Historiaclinica/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistoriaclinica(int id, HistoriaMapper historiaMapper)
        {
            if (id != historiaMapper.HistoriaId)
            {
                return BadRequest();
            }

            var historiaclinica = await _context.Historiaclinicas.FindAsync(id);

            if (historiaclinica == null)
            {
                return NotFound();
            }

            historiaclinica.MascotaId = historiaMapper.MascotaId;
            historiaclinica.EmpleadoId = historiaMapper.EmpleadoId;
            historiaclinica.FechaConsulta = DateTime.Now.Date; 
            historiaclinica.Sintomas = historiaMapper.Sintomas;
            historiaclinica.Diagnostico = historiaMapper.Diagnostico;
            historiaclinica.Tratamiento = historiaMapper.Tratamiento;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistoriaclinicaExists(id))
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

        // DELETE: api/Historiaclinica/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistoriaclinica(int id)
        {
            var historiaclinica = await _context.Historiaclinicas.FindAsync(id);
            if (historiaclinica == null)
            {
                return NotFound();
            }

            _context.Historiaclinicas.Remove(historiaclinica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistoriaclinicaExists(int id)
        {
            return _context.Historiaclinicas.Any(e => e.HistoriaId == id);
        }
    }
}