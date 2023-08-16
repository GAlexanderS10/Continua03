using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public partial class Historiaclinica
    {
        public int HistoriaId { get; set; }
        public int MascotaId { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string Sintomas { get; set; } = null!;
        public string Diagnostico { get; set; } = null!;
        public string Tratamiento { get; set; } = null!;

        [JsonIgnore]
        public virtual Empleado Empleado { get; set; } = null!;

        [JsonIgnore]
        public virtual Mascotum Mascota { get; set; } = null!;
    }
}
