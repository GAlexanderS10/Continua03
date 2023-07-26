namespace BackEnd.Mappers
{
    public class CitaMappercs
    {
        public int NroCita { get; set; }
        public int ClienteId { get; set; }
        public int MascotaId { get; set; }
        public string TipoServicio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaCita { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; }
    }
}
