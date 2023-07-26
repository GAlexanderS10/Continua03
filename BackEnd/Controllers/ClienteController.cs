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
            // Ordenar los clientes por ClienteId en orden descendente
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
            var existingCliente = _context.Clientes.FirstOrDefault(c => c.Dni == cliente.Dni);

            if (existingCliente != null)
            {
                // El DNI ya existe, devolver un mensaje de error
                return BadRequest("El cliente con este DNI ya existe en la base de datos.");
            }

            // El DNI no existe, agregar el cliente a la base de datos
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteId }, cliente);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public IActionResult PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.ClienteId)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;
            _context.SaveChanges();

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
