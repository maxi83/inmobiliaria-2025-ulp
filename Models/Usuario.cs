using System.ComponentModel.DataAnnotations;

namespace InmobiliariaUlP_2025.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required]
        public string Rol { get; set; } = ""; // "Administrador" o "Empleado"
    }
}
