namespace BackEnd.ViewModels
{
    public class MascotaViewModel
    {
        public string Nombre { get; set; }
        public string TipoMascota { get; set; }
        public string Raza { get; set; }
        public string Sexo { get; set; }
        public string Color { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public IFormFile Foto { get; set; }
        public int ClienteId { get; set; }
    }
}
