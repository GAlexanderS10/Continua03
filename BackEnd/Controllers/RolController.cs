using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public RolController(DBVETPETContext context)
        {
            _context = context;
        }

        // GET: api/Rols
        [HttpGet]
        public ActionResult<IEnumerable<Rol>> GetRols()
        {
            return _context.Rols.ToList();
        }

        // GET: api/Rols/5
        [HttpGet("{id}")]
        public ActionResult<Rol> GetRol(int id)
        {
            var rol = _context.Rols.Find(id);

            if (rol == null)
            {
                return NotFound();
            }

            return rol;
        }

        // POST: api/Rols
        [HttpPost]
        public ActionResult<Rol> PostRol(Rol rol)
        {
            // Verificar si ya existe un rol con el mismo tipo en la base de datos
            var existingRol = _context.Rols.FirstOrDefault(r => r.Tipo == rol.Tipo);

            if (existingRol != null)
            {
                // Si ya existe un rol con el mismo tipo, retornar un error indicando que el rol ya existe
                return BadRequest("Ya existe un rol con el mismo tipo.");
            }

            // Si no existe un rol con el mismo tipo, agregar el nuevo rol a la base de datos
            _context.Rols.Add(rol);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetRol), new { id = rol.RolId }, rol);
        }

        // PUT: api/Rols/5
        [HttpPut("{id}")]
        public ActionResult<Rol> ActualizarRol(int id, [FromBody] Rol rolActualizado)
        {
            // Buscar el rol existente en la base de datos por su ID
            var rolExistente = _context.Rols.Find(id);

            if (rolExistente == null)
            {
                // Si no se encuentra el rol, retornar un error indicando que el rol no existe
                return NotFound("No se encontró ningún rol con el ID proporcionado.");
            }

            // Verificar si ya existe otro rol con el mismo tipo en la base de datos (excepto el rol actual)
            var existingRol = _context.Rols.FirstOrDefault(r => r.Tipo == rolActualizado.Tipo && r.RolId != id);

            if (existingRol != null)
            {
                // Si ya existe otro rol con el mismo tipo, retornar un error indicando que el rol ya existe
                return BadRequest("Ya existe un rol con el mismo tipo.");
            }

            // Actualizar el rol existente con los nuevos datos del rol actualizado
            rolExistente.Tipo = rolActualizado.Tipo;

            // Guardar los cambios en la base de datos
            _context.SaveChanges();

            return Ok(rolExistente);
        }


        // DELETE: api/Rols/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRol(int id)
        {
            var rol = _context.Rols.Find(id);
            if (rol == null)
            {
                return NotFound();
            }

            _context.Rols.Remove(rol);
            _context.SaveChanges();

            return NoContent();
        }

        private bool RolExists(int id)
        {
            return _context.Rols.Any(e => e.RolId == id);
        }
    }
}