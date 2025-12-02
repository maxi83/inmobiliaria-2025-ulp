using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration)
            : base(configuration)
        {
        }

        // ============================================
        // CREAR
        // ============================================
        public int Alta(Propietario p)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Propietarios (Dni, Apellido, Nombre, Email, Telefono)
                VALUES (@dni, @apellido, @nombre, @correo, @telefono);
                SELECT LAST_INSERT_ID();";

            command.Parameters.AddWithValue("@dni", p.Dni);
            command.Parameters.AddWithValue("@apellido", p.Apellido);
            command.Parameters.AddWithValue("@nombre", p.Nombre);
            command.Parameters.AddWithValue("@correo", p.Email);
            command.Parameters.AddWithValue("@telefono", p.Telefono);

            connection.Open();
            int id = System.Convert.ToInt32(command.ExecuteScalar());
            p.Id = id;

            return id;
        }

        // ============================================
        // OBTENER TODOS
        // ============================================
        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono FROM Propietarios;";

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Propietario
                {
                    Id = reader.GetInt32(0),
                    Dni = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Nombre = reader.GetString(3),
                    Email = reader.GetString(4),
                    Telefono = reader.GetString(5)
                });
            }

            return lista;
        }

        // ============================================
        // BUSCAR POR ID
        // ============================================
        public Propietario? Buscar(int id)
        {
            Propietario? p = null;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT Id, Dni, Apellido, Nombre, Email, Telefono
                FROM Propietarios
                WHERE Id = @id;";

            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                p = new Propietario
                {
                    Id = reader.GetInt32(0),
                    Dni = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Nombre = reader.GetString(3),
                    Email = reader.GetString(4),
                    Telefono = reader.GetString(5)
                };
            }

            return p;
        }

        // ============================================
        // MODIFICAR
        // ============================================
        public int Modificacion(Propietario p)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Propietarios SET
                    Dni=@dni, Apellido=@apellido, Nombre=@nombre,
                    Email=@correo, Telefono=@telefono
                WHERE Id=@id;";

            command.Parameters.AddWithValue("@dni", p.Dni);
            command.Parameters.AddWithValue("@apellido", p.Apellido);
            command.Parameters.AddWithValue("@nombre", p.Nombre);
            command.Parameters.AddWithValue("@correo", p.Email);
            command.Parameters.AddWithValue("@telefono", p.Telefono);
            command.Parameters.AddWithValue("@id", p.Id);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        // ============================================
        // ELIMINAR
        // ============================================
        public int Baja(int id)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"DELETE FROM Propietarios WHERE Id=@id;";
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}
