using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public partial class Mascotum
    {
        public Mascotum()
        {
            Cita = new HashSet<Citum>();
            Historiaclinicas = new HashSet<Historiaclinica>();
        }

        public int MascotaId { get; set; }
        public string Nombre { get; set; } = null!;
        public string TipoMascota { get; set; } = null!;
        public string Raza { get; set; } = null!;
        public string Sexo { get; set; } = null!;
        public string Color { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
        public int ClienteId { get; set; }

        [JsonIgnore]
        public virtual Cliente Cliente { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Citum> Cita { get; set; }
        [JsonIgnore]
        public virtual ICollection<Historiaclinica> Historiaclinicas { get; set; }
    }
}
