using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper;
using BCrypt.Net;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DBVETPETContext _context;
        private readonly IMapper _mapper;

        public UsuarioController(DBVETPETContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("obtenerusuarios")]
        public async Task<ActionResult<List<Usuario>>> ObtenerUsuarios()
        {
            var usuarios = await _context.Usuarios.OrderByDescending(u => u.UsuarioId).ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("buscarusuario/{nombreUsuario}")]
        public async Task<ActionResult<Usuario>> BuscarUsuarioPorNombre(string nombreUsuario)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UserName == nombreUsuario);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost("crearusuario")]
        public async Task<ActionResult<int>> CrearUsuario([FromBody] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Encriptar la contraseña antes de almacenarla en la base de datos
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

                // Agregamos el nuevo usuario a la tabla Usuarios.
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Creamos un nuevo cliente y mapeamos los campos en común desde el usuario.
                var cliente = _mapper.Map<Cliente>(usuario);
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                // Devolvemos el UsuarioId recién creado
                return Ok(usuario.UsuarioId);
            }
            return BadRequest(ModelState);
        }


        [HttpPut("actualizarusuario/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Encriptar la contraseña antes de almacenarla en la base de datos
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteId == id);

                if (cliente == null)
                {
                    return NotFound();
                }

                // Actualizar campos específicos del cliente
                cliente.Nombres = usuario.Nombres;
                cliente.Apellidos = usuario.Apellidos;
                cliente.Dni = usuario.Dni;
                cliente.Celular = usuario.Celular;
                cliente.Email = usuario.Email;
                // Puedes agregar más campos según sea necesario

                _context.Entry(usuario).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("eliminarusuario/{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}