using System;
using System.Collections.Generic;

namespace BackEnd.Models
{
    public partial class Cargo
    {
        public int CargoId { get; set; }
        public string Cargo1 { get; set; } = null!;
        public string? Especialidad { get; set; }
        public decimal Sueldo { get; set; }

        public virtual Empleado? Empleado { get; set; }
    }
}
