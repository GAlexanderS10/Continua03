namespace BackEnd.ViewModels
{
    public class HistoriaViewModel
    {
        public int HistoriaId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public DateTime FechaConsulta { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
    }
}
