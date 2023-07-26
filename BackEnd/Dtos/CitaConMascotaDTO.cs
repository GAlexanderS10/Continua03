namespace BackEnd.Dtos
{
    public class CitaConMascotaDTO
    {
        public int NroCita { get; set; }
        public string NombreMascota { get; set; }
        public string TipoServicio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaCita { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
    }
}
