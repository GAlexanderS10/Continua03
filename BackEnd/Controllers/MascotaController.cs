using BackEnd.Dtos;
using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MascotaController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public MascotaController(DBVETPETContext context)
        {
            _context = context;
        }

        // GET: api/Mascotum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mascotum>>> GetMascota()
        {
            return await _context.Mascota.ToListAsync();
        }

        // GET: api/Mascotum/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mascotum>> GetMascotum(int id)
        {
            var mascotum = await _context.Mascota.FindAsync(id);

            if (mascotum == null)
            {
                return NotFound();
            }

            return mascotum;
        }

        [HttpPost("buscarPorDni")]
        public async Task<ActionResult<List<Mascotum>>> BuscarMascotasPorDni([FromBody] BuscarCitaDto buscarMascotasDTO)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Mascota)
                .FirstOrDefaultAsync(c => c.Dni == buscarMascotasDTO.DniCliente);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado");
            }

            var mascotasDelCliente = cliente.Mascota.ToList();
            return Ok(mascotasDelCliente);
        }

        [HttpGet("cliente/{id}")]
        public async Task<ActionResult<List<Mascotum>>> GetMascotasPorIdCliente(int id)
        {
            var mascotas = await _context.Mascota
                .Where(m => m.ClienteId == id)
                .OrderByDescending(m => m.MascotaId)
                .ToListAsync();

            if (mascotas == null || mascotas.Count == 0)
            {
                return NotFound();
            }

            return mascotas;
        }




        [HttpPost]
        public async Task<ActionResult<Mascotum>> PostMascota([FromForm] MascotaViewModel mascotaViewModel)
        {
            // Verificar si el cliente existe en la base de datos
            var cliente = await _context.Clientes.FindAsync(mascotaViewModel.ClienteId);
            if (cliente == null)
            {
                return BadRequest("El cliente especificado no existe.");
            }

            // Crear una nueva instancia de Mascotum y asignar los valores del ViewModel
            var mascotum = new Mascotum
            {
                Nombre = mascotaViewModel.Nombre,
                TipoMascota = mascotaViewModel.TipoMascota,
                Raza = mascotaViewModel.Raza,
                Sexo = mascotaViewModel.Sexo,
                Color = mascotaViewModel.Color,
                FechaNacimiento = mascotaViewModel.FechaNacimiento,
                ClienteId = mascotaViewModel.ClienteId // Asignar el ClienteId correctamente
            };

            // Guardar la imagen en el servidor si existe FotoFile
            if (mascotaViewModel.Foto != null && mascotaViewModel.Foto.Length > 0)
            {
                // Guardar la imagen en la carpeta "Uploads" en el directorio actual del proyecto
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, mascotaViewModel.Foto.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mascotaViewModel.Foto.CopyToAsync(stream);
                }

                // Asignar el nombre del archivo a la propiedad "Foto"
                mascotum.Foto = mascotaViewModel.Foto.FileName;
            }

            _context.Mascota.Add(mascotum);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMascota), new { id = mascotum.MascotaId }, mascotum);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutMascotum(int id, [FromForm] MascotaViewModel mascotaViewModel)
        {
            // Verificar si el cliente existe en la base de datos
            var cliente = await _context.Clientes.FindAsync(mascotaViewModel.ClienteId);
            if (cliente == null)
            {
                return BadRequest("El cliente especificado no existe.");
            }

            // Obtener la mascota existente de la base de datos
            var mascotum = await _context.Mascota.FindAsync(id);
            if (mascotum == null)
            {
                return NotFound();
            }

            // Actualizar las propiedades de la mascota con los valores del ViewModel
            mascotum.Nombre = mascotaViewModel.Nombre;
            mascotum.TipoMascota = mascotaViewModel.TipoMascota;
            mascotum.Raza = mascotaViewModel.Raza;
            mascotum.Sexo = mascotaViewModel.Sexo;
            mascotum.Color = mascotaViewModel.Color;
            mascotum.FechaNacimiento = mascotaViewModel.FechaNacimiento;
            mascotum.ClienteId = mascotaViewModel.ClienteId;

            // Si se proporciona una nueva imagen, guardarla en el servidor
            if (mascotaViewModel.Foto != null && mascotaViewModel.Foto.Length > 0)
            {
                // Guardar la imagen en la carpeta "Uploads" en el directorio actual del proyecto
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, mascotaViewModel.Foto.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mascotaViewModel.Foto.CopyToAsync(stream);
                }

                // Asignar el nombre del archivo a la propiedad "Foto"
                mascotum.Foto = mascotaViewModel.Foto.FileName;
            }

            // Guardar los cambios en la base de datos
            _context.Entry(mascotum).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/Mascotum/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMascotum(int id)
        {
            var mascotum = await _context.Mascota.FindAsync(id);
            if (mascotum == null)
            {
                return NotFound();
            }

            _context.Mascota.Remove(mascotum);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MascotumExists(int id)
        {
            return _context.Mascota.Any(e => e.MascotaId == id);
        }
    }
}