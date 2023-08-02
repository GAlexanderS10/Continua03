namespace BackEnd.Mappers
{
    public class HistoriaMapper
    {
        public int HistoriaId { get; set; }
        public int MascotaId { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string Sintomas { get; set; } = null!;
        public string Diagnostico { get; set; } = null!;
        public string Tratamiento { get; set; } = null!;
    }
}
