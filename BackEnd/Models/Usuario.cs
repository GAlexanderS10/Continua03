using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            Rols = new HashSet<Rol>();
        }

        public int UsuarioId { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public string Celular { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Rol> Rols { get; set; }
    }
}
