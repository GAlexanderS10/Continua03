using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public partial class Empleado
    {
        public Empleado()
        {
            Historiaclinicas = new HashSet<Historiaclinica>();
        }

        public int EmpleadoId { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? CargoId { get; set; }

        [JsonIgnore]
        public virtual Cargo? Cargo { get; set; }

        [JsonIgnore]
        public virtual ICollection<Historiaclinica> Historiaclinicas { get; set; }
    }
}
