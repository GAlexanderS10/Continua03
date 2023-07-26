using System;
using System.Collections.Generic;

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

        public virtual Cargo? Cargo { get; set; }
        public virtual ICollection<Historiaclinica> Historiaclinicas { get; set; }
    }
}
