namespace BackEnd.ViewModels
{
    public class ServicioViewModel
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal? Precio { get; set; }
        public IFormFile Imagen { get; set; }
    }
}
