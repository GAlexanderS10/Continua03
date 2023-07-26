using BackEnd.Models;
using BackEnd.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioController : ControllerBase
    {
        private readonly DBVETPETContext _context;

        public ServicioController(DBVETPETContext context)
        {
            _context = context;
        }

        //METODO PARA LISTAR TODOS LOS SERVICIOS REGISTRADOS 

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicio>>> GetServicio()
        {
            // Obtener los servicios ordenados de forma descendente según el servicioId
            List<Servicio> servicios = await _context.Servicios.OrderByDescending(s => s.ServicioId).ToListAsync();

            return servicios;
        }

        // METODO PARA BUSCAR POR EL ID EL REGISTRO  

        [HttpGet("{id}")]
        public async Task<ActionResult<Servicio>> GetServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }

            return servicio;
        }

        //METODO PARA REGISTRAR UN NUEVO SERVICIO

        [HttpPost]
        public async Task<ActionResult<Servicio>> PostServicio([FromForm] ServicioViewModel servicioViewModel)
        {
            // Verificar si ya existe un servicio con el mismo nombre
            var servicios = await _context.Servicios.ToListAsync();
            if (servicios.Any(s => string.Equals(s.Nombre, servicioViewModel.Nombre, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Ya existe un servicio con el mismo nombre.");
            }

            // Crear una nueva instancia de Servicio y asignar los valores del ViewModel
            var servicio = new Servicio
            {
                Nombre = servicioViewModel.Nombre,
                Descripcion = servicioViewModel.Descripcion,
                Precio = servicioViewModel.Precio,
            };

            // Guardar la imagen en el servidor si existe ImagenFile
            if (servicioViewModel.Imagen != null && servicioViewModel.Imagen.Length > 0)
            {
                // Guardar la imagen en la carpeta "Uploads" en el directorio actual del proyecto
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, servicioViewModel.Imagen.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await servicioViewModel.Imagen.CopyToAsync(stream);
                }

                // Asignar el nombre del archivo a la propiedad "Imagen"
                servicio.Imagen = servicioViewModel.Imagen.FileName;
            }

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServicio), new { id = servicio.ServicioId }, servicio);
        }

        //METODO PARA ACTUALIZAR LOS DATOS DE UN SERVICIO

        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicio(int id, [FromForm] ServicioViewModel servicioViewModel)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            // Actualizar las propiedades de la mascota con los valores del ViewModel
            servicio.Nombre = servicioViewModel.Nombre;
            servicio.Descripcion = servicioViewModel.Descripcion;
            servicio.Precio = servicioViewModel.Precio;

            // Si se proporciona una nueva imagen, guardarla en el servidor
            if (servicioViewModel.Imagen != null && servicioViewModel.Imagen.Length > 0)
            {
                // Guardar la imagen en la carpeta "Uploads" en el directorio actual del proyecto
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, servicioViewModel.Imagen.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await servicioViewModel.Imagen.CopyToAsync(stream);
                }

                // Asignar el nombre del archivo a la propiedad "Imagen"
                servicio.Imagen = servicioViewModel.Imagen.FileName;
            }

            // Guardar los cambios en la base de datos
            _context.Entry(servicio).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //METODO PARA ELIMINAR UN SERVICIO

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}