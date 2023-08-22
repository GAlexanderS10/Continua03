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
    public class EmpleadoController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public EmpleadoController(DBVETPETContext context)
        {
            _context = context;
        }

        // GET: api/Empleado
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            return await _context.Empleados.ToListAsync();
        }

        // GET: api/Empleado/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> GetEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound();
            }

            return empleado;
        }

        [HttpGet("ListarEmpleadosConCargo")]
        public async Task<ActionResult<IEnumerable<EmpleadoConCargo>>> ListarEmpleadosConCargo()
        {
            try
            {
                var empleadosConCargo = await _context.Empleados
                    .Include(e => e.Cargo)
                    .Select(e => new EmpleadoConCargo
                    {
                        EmpleadoId = e.EmpleadoId,
                        Nombres = e.Nombres,
                        Apellidos = e.Apellidos,
                        Dni = e.Dni,
                        Celular = e.Celular,
                        Email = e.Email,
                        Cargo1 = e.Cargo.Cargo1,
                        Especialidad = e.Cargo.Especialidad,
                        Sueldo = e.Cargo.Sueldo
                    })
                    .ToListAsync();

                return Ok(empleadosConCargo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener la lista de empleados con cargo: {ex.Message}");
            }
        }


        [HttpGet("buscar-por-dni/{dni}")]
        public async Task<ActionResult<Empleado>> GetEmpleadoByDni(string dni)
        {
            // Buscar al empleado por su número de DNI en la base de datos
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Dni == dni);

            if (empleado == null)
            {
                return NotFound();
            }

            return empleado;
        }

        // POST: api/Empleado
        [HttpPost]
        public async Task<ActionResult<Empleado>> PostEmpleado(EmpleadoMapper empleadoMapper)
        {
            // Verificar si ya existe un empleado con el mismo DNI o correo electrónico en la base de datos
            var existingDni = await _context.Empleados.FirstOrDefaultAsync(e => e.Dni == empleadoMapper.Dni);
            var existingEmail = await _context.Empleados.FirstOrDefaultAsync(e => e.Email == empleadoMapper.Email);

            if (existingDni != null)
            {
                // Si ya existe un empleado con el mismo DNI, retornar un error indicando que el DNI ya está registrado
                return BadRequest("El DNI ya está registrado.");
            }

            if (existingEmail != null)
            {
                // Si ya existe un empleado con el mismo correo electrónico, retornar un error indicando que el correo ya está registrado
                return BadRequest("El correo electrónico ya está registrado.");
            }

            var empleado = new Empleado
            {
                Nombres = empleadoMapper.Nombres,
                Apellidos = empleadoMapper.Apellidos,
                Dni = empleadoMapper.Dni,
                Celular = empleadoMapper.Celular,
                Email = empleadoMapper.Email,
                CargoId = empleadoMapper.CargoId
            };

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.EmpleadoId }, empleado);
        }

        // PUT: api/Empleado/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, EmpleadoMapper empleadoMapper)
        {
            // Verificar si el ID proporcionado coincide con el ID del empleado en el objeto empleado
            if (id != empleadoMapper.EmpleadoId)
            {
                return BadRequest();
            }

            // Verificar si ya existe un empleado con el mismo DNI o correo electrónico en la base de datos
            var existingDni = await _context.Empleados.FirstOrDefaultAsync(e => e.Dni == empleadoMapper.Dni && e.EmpleadoId != id);
            var existingEmail = await _context.Empleados.FirstOrDefaultAsync(e => e.Email == empleadoMapper.Email && e.EmpleadoId != id);

            if (existingDni != null)
            {
                // Si ya existe un empleado con el mismo DNI, retornar un error indicando que el DNI ya está registrado
                return BadRequest("El DNI ya está registrado.");
            }

            if (existingEmail != null)
            {
                // Si ya existe un empleado con el mismo correo electrónico, retornar un error indicando que el correo ya está registrado
                return BadRequest("El correo electrónico ya está registrado.");
            }

            // Obtener el empleado existente desde la base de datos
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound();
            }

            // Actualizar los campos del empleado existente con los valores del empleado enviado en el cuerpo de la solicitud
            empleado.Nombres = empleadoMapper.Nombres;
            empleado.Apellidos = empleadoMapper.Apellidos;
            empleado.Dni = empleadoMapper.Dni;
            empleado.Celular = empleadoMapper.Celular;
            empleado.Email = empleadoMapper.Email;
            empleado.CargoId = empleadoMapper.CargoId;

            // Guardar los cambios en la base de datos
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si ocurre una excepción de concurrencia, verificar si el empleado aún existe en la base de datos
                if (!EmpleadoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Retornar un resultado exitoso
            return NoContent();
        }

        // DELETE: api/Empleado/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.EmpleadoId == id);
        }
    }
}