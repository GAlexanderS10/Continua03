namespace BackEnd.Dtos
{
    public class EmpleadoConCargo
    {
        public int EmpleadoId { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Cargo1 { get; set; } = null!;
        public string? Especialidad { get; set; }
        public decimal Sueldo { get; set; }
    }
}
