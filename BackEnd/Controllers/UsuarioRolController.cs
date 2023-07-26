using BackEnd.Mappers;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioRolController : ControllerBase
    {
        private readonly DBVETPETContext _dbContext; // Reemplaza DbContext con tu clase que maneja la conexión a la base de datos

        public UsuarioRolController(DBVETPETContext dbContext)
        {
            _dbContext = dbContext;
        }

 


        // Endpoint para asignar un rol a un usuario
        [HttpPost("asignar-rol")]
        public async Task<IActionResult> AsignarRolUsuario([FromBody] AsignarRolUsuarioModel model)
        {
            try
            {
                // Buscar el usuario y el rol en la base de datos
                var usuarioExistente = await _dbContext.Usuarios.FindAsync(model.UsuarioId);
                var rolExistente = await _dbContext.Rols.FindAsync(model.RolId);

                if (usuarioExistente == null || rolExistente == null)
                {
                    return BadRequest("El usuario o el rol especificado no existe.");
                }

                // Asignar el rol al usuario y guardar los cambios en la base de datos
                usuarioExistente.Rols.Add(rolExistente);
                await _dbContext.SaveChangesAsync();

                return Ok("Rol asignado al usuario exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la solicitud.");
            }
        }

    }
}