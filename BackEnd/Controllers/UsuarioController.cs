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

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> ObtenerUsuarios()
        {
            var usuarios = await _context.Usuarios.OrderByDescending(u => u.UsuarioId).ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public ActionResult<Usuario> GetUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }


        [HttpGet("buscarporusername/{username}")]
        public ActionResult<Usuario> BuscarPorUserName(string username)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.UserName == username);

            if (usuario == null)
            {
                return NotFound("No se encontró ningún usuario con el username proporcionado.");
            }

            return usuario;
        }

        [HttpGet("buscar")]
        public ActionResult<IEnumerable<Usuario>> BuscarUsuario([FromQuery] string searchTerm)
        {
            // Obtener todos los usuarios desde la base de datos
            List<Usuario> usuarios = _context.Usuarios
                .OrderByDescending(u => u.UsuarioId)
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                usuarios = usuarios.Where(u =>
                    u.Dni.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return usuarios;
        }

        [HttpGet("Dni/{dni}")]
        public ActionResult<Usuario> GetClienteByDni(string dni)
        {
            var usuario = _context.Usuarios.FirstOrDefault(c => c.Dni == dni);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }



        // POST: api/Usuarios
        [HttpPost]
        public ActionResult<Usuario> PostUsuario(Usuario usuario)
        {
            // Verificar si ya existe un usuario con el mismo DNI, email o username en la base de datos
            var existingDni = _context.Usuarios.FirstOrDefault(u => u.Dni == usuario.Dni);
            var existingEmail = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
            var existingUsername = _context.Usuarios.FirstOrDefault(u => u.UserName == usuario.UserName);

            if (existingDni != null)
            {
                // Si ya existe un usuario con el mismo DNI, retornar un error indicando que el DNI ya está registrado
                return BadRequest("El DNI ya está registrado.");
            }

            if (existingEmail != null)
            {
                // Si ya existe un usuario con el mismo email, retornar un error indicando que el email ya está registrado
                return BadRequest("El email ya está registrado.");
            }

            if (existingUsername != null)
            {
                // Si ya existe un usuario con el mismo username, retornar un error indicando que el username ya está registrado
                return BadRequest("El username ya está registrado.");
            }

            // Cifrar la contraseña antes de almacenarla en la base de datos
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            usuario.Password = hashedPassword;

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuario);
        }

        [HttpPost("crearusuario")]
        public async Task<ActionResult<int>> CrearUsuario([FromBody] Usuario usuario)
        {
            // Verificar si ya existe un usuario con el mismo DNI, email o username en la base de datos
            var existingDni = await _context.Usuarios.FirstOrDefaultAsync(u => u.Dni == usuario.Dni);
            var existingEmail = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
            var existingUsername = await _context.Usuarios.FirstOrDefaultAsync(u => u.UserName == usuario.UserName);

            if (existingDni != null)
            {
                // Si ya existe un usuario con el mismo DNI, retornar un error indicando que el DNI ya está registrado
                ModelState.AddModelError("Dni", "El DNI ya está registrado.");
                return BadRequest(ModelState);
            }

            if (existingEmail != null)
            {
                // Si ya existe un usuario con el mismo email, retornar un error indicando que el email ya está registrado
                ModelState.AddModelError("Email", "El email ya está registrado.");
                return BadRequest(ModelState);
            }

            if (existingUsername != null)
            {
                // Si ya existe un usuario con el mismo username, retornar un error indicando que el username ya está registrado
                ModelState.AddModelError("UserName", "El username ya está registrado.");
                return BadRequest(ModelState);
            }

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


        // PUT: api/Usuarios/5
        [HttpPut("editarusuario/{id}")]
        public ActionResult<Usuario> EditarUsuario(int id, Usuario usuario)
        {
            // Verificar si el usuario con el id proporcionado existe en la base de datos
            var existingUsuario = _context.Usuarios.Find(id);
            if (existingUsuario == null)
            {
                return NotFound();
            }

            // Verificar si los campos DNI, email o username están siendo utilizados por otro usuario en la base de datos
            var existingDni = _context.Usuarios.FirstOrDefault(u => u.Dni == usuario.Dni && u.UsuarioId != id);
            var existingEmail = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email && u.UsuarioId != id);
            var existingUsername = _context.Usuarios.FirstOrDefault(u => u.UserName == usuario.UserName && u.UsuarioId != id);

            if (existingDni != null)
            {
                // Si el DNI está siendo utilizado por otro usuario, retornar un error indicando que el DNI ya está registrado
                return BadRequest("El DNI ya está registrado por otro usuario.");
            }

            if (existingEmail != null)
            {
                // Si el email está siendo utilizado por otro usuario, retornar un error indicando que el email ya está registrado
                return BadRequest("El email ya está registrado por otro usuario.");
            }

            if (existingUsername != null)
            {
                // Si el username está siendo utilizado por otro usuario, retornar un error indicando que el username ya está registrado
                return BadRequest("El username ya está registrado por otro usuario.");
            }

            // Actualizar los datos del usuario existente con la información proporcionada
            existingUsuario.Nombres = usuario.Nombres;
            existingUsuario.Apellidos = usuario.Apellidos;
            existingUsuario.Dni = usuario.Dni;
            existingUsuario.Celular = usuario.Celular;
            existingUsuario.Email = usuario.Email;
            existingUsuario.UserName = usuario.UserName;

            // Cifrar la contraseña antes de almacenarla en la base de datos si se proporcionó una nueva contraseña
            if (!string.IsNullOrEmpty(usuario.Password))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
                existingUsuario.Password = hashedPassword;
            }

            _context.SaveChanges();

            return Ok(existingUsuario);
        }


        [HttpPut("actualizarusuario/{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuario)
        {
            // Verificar si el usuario con el id proporcionado existe en la base de datos
            var existingUsuario = await _context.Usuarios.FindAsync(id);
            if (existingUsuario == null)
            {
                return NotFound();
            }

            // Verificar si el DNI, email o username están siendo utilizados por otro usuario en la base de datos
            var existingDni = await _context.Usuarios.FirstOrDefaultAsync(u => u.Dni == usuario.Dni && u.UsuarioId != id);
            var existingEmail = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email && u.UsuarioId != id);
            var existingUsername = await _context.Usuarios.FirstOrDefaultAsync(u => u.UserName == usuario.UserName && u.UsuarioId != id);

            if (existingDni != null)
            {
                // Si el DNI está siendo utilizado por otro usuario, retornar un error indicando que el DNI ya está registrado
                ModelState.AddModelError("Dni", "El DNI ya está registrado por otro usuario.");
                return BadRequest(ModelState);
            }

            if (existingEmail != null)
            {
                // Si el email está siendo utilizado por otro usuario, retornar un error indicando que el email ya está registrado
                ModelState.AddModelError("Email", "El email ya está registrado por otro usuario.");
                return BadRequest(ModelState);
            }

            if (existingUsername != null)
            {
                // Si el username está siendo utilizado por otro usuario, retornar un error indicando que el username ya está registrado
                ModelState.AddModelError("UserName", "El username ya está registrado por otro usuario.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                // Actualizar los datos del usuario existente con la información proporcionada
                existingUsuario.Nombres = usuario.Nombres;
                existingUsuario.Apellidos = usuario.Apellidos;
                existingUsuario.Dni = usuario.Dni;
                existingUsuario.Celular = usuario.Celular;
                existingUsuario.Email = usuario.Email;
                existingUsuario.UserName = usuario.UserName;
                existingUsuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

                await _context.SaveChangesAsync();

                return NoContent();
            }

            return BadRequest(ModelState);
        }


        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}