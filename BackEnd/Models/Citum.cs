using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public partial class Citum
    {
        public int NroCita { get; set; }
        public int ClienteId { get; set; }
        public int MascotaId { get; set; }
        public string TipoServicio { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaCita { get; set; }
        public string Hora { get; set; } = null!;
        public string Estado { get; set; } = null!;

        [JsonIgnore]
        public virtual Cliente Cliente { get; set; } = null!;
        [JsonIgnore]
        public virtual Mascotum Mascota { get; set; } = null!;
    }
}
