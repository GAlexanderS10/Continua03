using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public partial class Rol
    {
        public Rol()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public int RolId { get; set; }
        public string Tipo { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
