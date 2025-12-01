using System.ComponentModel.DataAnnotations;

namespace InmobiliariaUlP_2025.Models
{
    // Un inquilino hereda DNI, Nombre, Apellido, Email, Telefono
    // desde DatosPersonales.
    public class Inquilino : DatosPersonales
    {
        [Key] // Clave primaria
        public int Id { get; set; }

        // Podríamos agregar más cosas después,
        // pero por ahora esto es suficiente.
    }
}
