using BackEnd.Dtos;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitaViewModel>>> GetCitas()
        {
            var citas = await _context.Cita
                .Include(c => c.Cliente)
                .Include(c => c.Mascota)
                .OrderBy(c => c.FechaCita)
                .Select(c => new CitaViewModel
                {
                    NroCita = c.NroCita,
                    Dni = c.Cliente.Dni,
                    Mascota = c.Mascota.Nombre,
                    Servicio = c.TipoServicio,
                    FechaRegistro = c.FechaRegistro,
                    FechaCita = c.FechaCita,
                    Hora = c.Hora,
                    Estado = c.Estado
                })
                .ToListAsync();

            return citas;
        }


        [HttpGet("buscar")]
        public ActionResult<IEnumerable<CitaViewModel>> BuscarCitasPorDni([FromQuery] string searchTerm)
        {
            // Obtener todas las citas desde la base de datos
            List<Citum> citas = _context.Cita
                .Include(c => c.Cliente)
                .Include(c => c.Mascota)
                .OrderByDescending(c => c.NroCita)
                .ToList();

            // Realizar la búsqueda en la lista de citas en memoria
            List<CitaViewModel> citasFiltradas = citas
                .Where(c =>
                    c.Cliente.Dni.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(c => new CitaViewModel
                {
                    NroCita = c.NroCita,
                    Dni = c.Cliente.Dni,
                    Mascota = c.Mascota.Nombre,
                    Servicio = c.TipoServicio,
                    FechaRegistro = c.FechaRegistro,
                    FechaCita = c.FechaCita,
                    Hora = c.Hora,
                    Estado = c.Estado
                })
                .ToList();

            return citasFiltradas;
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
        [HttpGet("obtenerCitasConMascota/{idCliente}")]
        public async Task<ActionResult<List<CitaConMascotaDTO>>> ObtenerCitasConMascota(int idCliente)
        {
            var citasConMascota = await _context.Cita
                .Include(c => c.Mascota)
                .Where(c => c.ClienteId == idCliente) // Filtrar por el ID del cliente
                .Select(c => new CitaConMascotaDTO
                {
                    NroCita = c.NroCita,
                    NombreMascota = c.Mascota.Nombre,
                    TipoServicio = c.TipoServicio,
                    FechaRegistro = c.FechaRegistro,
                    FechaCita = c.FechaCita,
                    Hora = c.Hora,
                    Estado = c.Estado,
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
                    FechaCita = citaDTO.FechaCita.Date,
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


        [HttpPut("{nroCita}")]
        public IActionResult ActualizarCita(int nroCita, [FromBody] CitaViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Datos de la cita inválidos");
            }

            // Validar el DNI del cliente y obtener el ClienteId
            var cliente = _context.Clientes.SingleOrDefault(c => c.Dni == model.Dni);
            if (cliente == null)
            {
                return NotFound($"Cliente con DNI {model.Dni} no encontrado");
            }

            // Validar el nombre de la mascota y obtener el MascotaId
            var mascota = _context.Mascota.SingleOrDefault(m => m.Nombre == model.Mascota);
            if (mascota == null)
            {
                return NotFound($"Mascota con nombre {model.Mascota} no encontrada");
            }

            // Verificar si la cita con el nroCita existe en la base de datos
            var citaExistente = _context.Cita.SingleOrDefault(c => c.NroCita == nroCita);
            if (citaExistente == null)
            {
                return NotFound($"Cita con NroCita {nroCita} no encontrada");
            }

            // Actualizar los campos necesarios del objeto citaExistente con los valores del ViewModel
            citaExistente.ClienteId = cliente.ClienteId;
            citaExistente.MascotaId = mascota.MascotaId;
            citaExistente.TipoServicio = model.Servicio;
            citaExistente.FechaRegistro = DateTime.Now.Date;
            citaExistente.FechaCita = model.FechaCita;
            citaExistente.Hora = model.Hora;
            citaExistente.Estado = model.Estado;

            // Guardar los cambios en la base de datos
            try
            {
                _context.SaveChanges();
                return Ok("Cita actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la cita: {ex.Message}");
            }
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
