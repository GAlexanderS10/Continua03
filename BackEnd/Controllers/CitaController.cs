using BackEnd.Dtos;
using BackEnd.Mappers;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitaController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public CitaController(DBVETPETContext context)
        {
            _context = context;
        }

        [HttpGet("obtenerCitasDescendente")]
        public async Task<ActionResult<List<Citum>>> ObtenerCitasDescendente()
        {
            var citas = await _context.Cita.OrderByDescending(c => c.NroCita).ToListAsync();
            return Ok(citas);
        }

        [HttpGet("obtenerCita/{nroCita}")]
        public async Task<ActionResult<Citum>> ObtenerCitaPorNro(int nroCita)
        {
            var cita = await _context.Cita.FindAsync(nroCita);

            if (cita == null)
            {
                return NotFound();
            }

            return Ok(cita);
        }

        [HttpGet("obtenerCitasConMascota")]
        public async Task<ActionResult<List<CitaConMascotaDTO>>> ObtenerCitasConMascota()
        {
            var citasConMascota = await _context.Cita
                .Include(c => c.Mascota) 
                .Select(c => new CitaConMascotaDTO
                {
                    NroCita = c.NroCita,
                    TipoServicio = c.TipoServicio,
                    FechaRegistro = c.FechaRegistro,
                    FechaCita = c.FechaCita,
                    Hora = c.Hora,
                    Estado = c.Estado,
                    NombreMascota = c.Mascota.Nombre, 
                })
                .OrderByDescending(c => c.NroCita)
                .ToListAsync();

            return Ok(citasConMascota);
        }


        [HttpPost("buscarPorDni")]
        public async Task<ActionResult<List<Citum>>> BuscarCitaPorDni([FromBody] BuscarCitaDto buscarCitaDTO)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Cita)
                .FirstOrDefaultAsync(c => c.Dni == buscarCitaDTO.DniCliente);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado");
            }

            var citasDelCliente = cliente.Cita.ToList();
            return Ok(citasDelCliente);
        }


        [HttpPost("crearCita")]
        public async Task<ActionResult<Citum>> CrearCita([FromBody] CitaMappercs citaDTO)
        {
            if (ModelState.IsValid)
            {
                var cita = new Citum
                {
                    NroCita = citaDTO.NroCita,
                    ClienteId = citaDTO.ClienteId,
                    MascotaId = citaDTO.MascotaId,
                    TipoServicio = citaDTO.TipoServicio,
                    FechaRegistro = DateTime.Now, 
                    FechaCita = citaDTO.FechaCita,
                    Hora = citaDTO.Hora,
                    Estado = citaDTO.Estado
                };

                _context.Cita.Add(cita);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObtenerCitaPorNro), new { nroCita = cita.NroCita }, cita);
            }

            return BadRequest(ModelState);
        }


        [HttpPut("actualizarCita/{nroCita}")]
        public async Task<IActionResult> ActualizarCita(int nroCita, [FromBody] CitaMappercs citaDTO)
        {
            if (nroCita != citaDTO.NroCita)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var cita = await _context.Cita.FindAsync(nroCita);

                if (cita == null)
                {
                    return NotFound();
                }

                cita.ClienteId = citaDTO.ClienteId;
                cita.MascotaId = citaDTO.MascotaId;
                cita.TipoServicio = citaDTO.TipoServicio;
                cita.FechaRegistro = citaDTO.FechaRegistro;
                cita.FechaCita = citaDTO.FechaCita;
                cita.Hora = citaDTO.Hora;
                cita.Estado = citaDTO.Estado;

                _context.Entry(cita).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }

            return BadRequest(ModelState);
        }




        [HttpDelete("eliminarCita/{nroCita}")]
        public async Task<IActionResult> EliminarCita(int nroCita)
        {
            var cita = await _context.Cita.FindAsync(nroCita);

            if (cita == null)
            {
                return NotFound();
            }

            _context.Cita.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
