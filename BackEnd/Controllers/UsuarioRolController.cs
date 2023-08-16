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
    public class UsuarioRolController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public UsuarioRolController(DBVETPETContext context)
        {
            _context = context;
        }

        [HttpGet("{tipoRol}")]
        public IEnumerable<Usuario> GetUsuariosPorTipoRol(string tipoRol)
        {
            var usuariosPorRol = _context.Usuarios
                .Where(u => u.Rols.Any(r => r.Tipo.Contains(tipoRol)))
                .ToList();

            return usuariosPorRol;
        }


        [HttpGet("RolesDescendentes/{usuarioId}")]
        public IActionResult GetRolesDescendentesPorUsuarioId(int usuarioId)
        {
            var rolesDescendentes = _context.Rols
                .Where(r => r.Usuarios.Any(u => u.UsuarioId == usuarioId))
                .OrderByDescending(r => r.RolId)
                .ToList();

            var usuario = _context.Usuarios.Find(usuarioId);

            if (usuario == null)
            {
                return NotFound("El usuario especificado no existe.");
            }

            var response = new
            {
                UsuarioId = usuario.UsuarioId,
                RolesDescendentes = rolesDescendentes
            };

            return Ok(response);
        }




        // POST: api/UsuarioRol
        [HttpPost]
        public IActionResult PostUsuarioRol(UsuarioRolModel model)
        {
            var usuario = _context.Usuarios.Find(model.UsuarioId);
            var rol = _context.Rols.Find(model.RolId);

            if (usuario == null || rol == null)
            {
                return NotFound("El usuario o el rol especificado no existe.");
            }

            usuario.Rols.Add(rol);
            _context.SaveChanges();

            return Ok("Rol asignado al usuario exitosamente.");
        }

        // PUT: api/UsuarioRol
        [HttpPut]
        public IActionResult PutUsuarioRol(UsuarioRolModel model)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Rols)
                .FirstOrDefault(u => u.UsuarioId == model.UsuarioId);

            var rol = _context.Rols.Find(model.RolId);

            if (usuario == null || rol == null)
            {
                return NotFound("El usuario o el rol especificado no existe.");
            }

            // Eliminar el rol anterior (si existe) y agregar el nuevo rol
            usuario.Rols.Clear();
            usuario.Rols.Add(rol);

            _context.SaveChanges();

            return Ok("Rol del usuario actualizado exitosamente.");
        }


        // DELETE: api/UsuarioRol
        [HttpDelete]
        public IActionResult DeleteUsuarioRol(UsuarioRolModel model)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Rols)
                .FirstOrDefault(u => u.UsuarioId == model.UsuarioId);

            var rol = usuario?.Rols.FirstOrDefault(r => r.RolId == model.RolId);

            if (usuario == null || rol == null)
            {
                return NotFound("El usuario o el rol especificado no existe.");
            }

            usuario.Rols.Remove(rol);
            _context.SaveChanges();

            return Ok("Rol eliminado del usuario exitosamente.");
        }

    }
}