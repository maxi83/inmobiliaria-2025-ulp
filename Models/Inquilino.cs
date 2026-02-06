using MySqlConnector;

namespace InmobiliariaUlP_2025.Models
{
    public class Inquilino : DatosPersonales
    {
        public Inquilino() { }

        public Inquilino(MySqlDataReader reader)
        {
            Id = reader.GetInt32("Id");
            Dni = reader.GetString("Dni");
            Nombre = reader.GetString("Nombre");
            Apellido = reader.GetString("Apellido");
            Telefono = reader.GetString("Telefono");
            Email = reader.GetString("Email");
        }

        public int Id { get; set; }
    }
}
