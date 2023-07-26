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

        //METODO PARA LISTAR TODO LOS ROLES QUE EXISTEN EN LA BASE DE DATOS

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            return await _context.Rols.OrderByDescending(r => r.RolId).ToListAsync();
        }

        //METODO PARA BUSCAR EL ROL POR SU ID

        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Rols.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            return rol;
        }

        // METODO PARA CREAR UN NUEVO ROL 

        [HttpPost]
        public ActionResult<Rol> CreateRol(Rol rol)
        {
            try
            {
                // Convertir el tipo a minúsculas para realizar la comparación sin distinguir mayúsculas y minúsculas
                string tipoLowerCase = rol.Tipo.ToLower();

                // Verificar si ya existe un rol con el mismo tipo (ignorando mayúsculas y minúsculas)
                bool rolExists = _context.Rols.Any(r => r.Tipo.ToLower() == tipoLowerCase);

                if (rolExists)
                {
                    return BadRequest("El rol ya existe.");
                }

                // Agregar el nuevo rol y guardar cambios en la base de datos
                _context.Rols.Add(rol);
                _context.SaveChanges();

                return CreatedAtAction("GetRol", new { id = rol.RolId }, rol);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // METODO PARA ACTUALIZAR EL ROL

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRol(int id, Rol rol)
        {
            if (id != rol.RolId)
            {
                return BadRequest();
            }

            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
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

        // METODO PARA ELIMINAR EL ROL

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Rols.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            _context.Rols.Remove(rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //METODO PARA COMPROBAR SI EL ROL YA EXISTE EN LA BASE DE DATOS

        private bool RolExists(int id)
        {
            return _context.Rols.Any(e => e.RolId == id);
        }
    }
}






