using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public ClienteController(DBVETPETContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Cliente>> GetClientes()
        {
    
            var clientes = _context.Clientes.OrderByDescending(c => c.ClienteId).ToList();
            return clientes;
        }

        [HttpGet("{id}")]
        public ActionResult<Cliente> GetCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("buscar")]
        public ActionResult<IEnumerable<Cliente>> BuscarClientePorDni([FromQuery] string searchTerm)
        {
        
            List<Cliente> clientes = _context.Clientes
                .OrderByDescending(c => c.ClienteId)
                .ToList();

            clientes = clientes.Where(c => c.Dni.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            return clientes;
        }


        [HttpGet("Dni/{dni}")]
        public ActionResult<Cliente> GetClienteByDni(string dni)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Dni == dni);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpPost]
        public ActionResult<Cliente> PostCliente(Cliente cliente)
        {
            // Verificar si el DNI ya existe en la base de datos
            var existingDni = _context.Clientes.FirstOrDefault(c => c.Dni == cliente.Dni);

            if (existingDni != null)
            {
                // El DNI ya existe, devolver un mensaje de error
                return BadRequest("El cliente con este DNI ya existe en la base de datos.");
            }

            // Verificar si el correo electrónico ya existe en la base de datos
            var existingEmail = _context.Clientes.FirstOrDefault(c => c.Email == cliente.Email);

            if (existingEmail != null)
            {
                // El correo electrónico ya existe, devolver un mensaje de error
                return BadRequest("El cliente con este correo electrónico ya existe en la base de datos.");
            }

            // Ni el DNI ni el correo electrónico existen, agregar el cliente a la base de datos
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteId }, cliente);
        }


        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public IActionResult PutCliente(int id, Cliente cliente)
        {
            // Verificar si el ID proporcionado coincide con el ID del cliente en el objeto cliente
            if (id != cliente.ClienteId)
            {
                return BadRequest();
            }

            // Verificar si ya existe un cliente con el mismo DNI o correo electrónico en la base de datos
            var existingDni = _context.Clientes.FirstOrDefault(c => c.Dni == cliente.Dni && c.ClienteId != id);
            var existingEmail = _context.Clientes.FirstOrDefault(c => c.Email == cliente.Email && c.ClienteId != id);

            if (existingDni != null)
            {
                // Si ya existe un cliente con el mismo DNI, retornar un error indicando que el DNI ya está registrado
                return BadRequest("El DNI ya está registrado por otro cliente.");
            }

            if (existingEmail != null)
            {
                // Si ya existe un cliente con el mismo correo electrónico, retornar un error indicando que el correo ya está registrado
                return BadRequest("El correo electrónico ya está registrado por otro cliente.");
            }

            // Obtener el cliente existente desde la base de datos
            var clienteExistente = _context.Clientes.Find(id);

            if (clienteExistente == null)
            {
                return NotFound();
            }

            // Actualizar los campos del cliente existente con los valores del cliente enviado en el cuerpo de la solicitud
            clienteExistente.Nombres = cliente.Nombres;
            clienteExistente.Apellidos = cliente.Apellidos;
            clienteExistente.Dni = cliente.Dni;
            clienteExistente.Celular = cliente.Celular;
            clienteExistente.Email = cliente.Email;

            // Guardar los cambios en la base de datos
            _context.SaveChanges();

            // Retornar un resultado exitoso
            return NoContent();
        }


        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();

            return NoContent();
        }
    }
}