namespace BackEnd.Mappers
{
    public class EmpleadoMapper
    {
        public int EmpleadoId { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? CargoId { get; set; }
    }
}
