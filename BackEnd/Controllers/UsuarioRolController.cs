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
    }
}