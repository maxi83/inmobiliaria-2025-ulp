using System.ComponentModel.DataAnnotations;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Models
{
    public class Propietario : DatosPersonales
    {
        public Propietario() { }

        public Propietario(MySqlDataReader reader)
        {
            Id = reader.GetInt32("Id");
            Dni = reader.GetString("Dni");
            Apellido = reader.GetString("Apellido");
            Nombre = reader.GetString("Nombre");
            Email = reader.GetString("Email");
            Telefono = reader.GetString("Telefono");
        }

        [Key]
        public int Id { get; set; }
    }
}
