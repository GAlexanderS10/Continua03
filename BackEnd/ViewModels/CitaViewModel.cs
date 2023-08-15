namespace BackEnd.ViewModels
{
    public class CitaViewModel
    {
        public int NroCita { get; set; }

        public string Dni { get; set; } = null!;

        public string Mascota { get; set; } = null!;

        public string Servicio { get; set; } = null!;

        public DateTime FechaRegistro { get; set; }

        public DateTime FechaCita { get; set; }

        public string Hora { get; set; } = null!;

        public string Estado { get; set; } = null!;
    }
}
