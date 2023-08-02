using System.ComponentModel.DataAnnotations;

namespace BackEnd.ViewModels
{
    public class UsuarioRolModel
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int RolId { get; set; }
    }
}
