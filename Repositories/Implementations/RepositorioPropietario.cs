using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

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
            try
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
                int id = Convert.ToInt32(command.ExecuteScalar());
                p.Id = id;
                return id;
            }
            catch (MySqlException)
            {
                // DNI o Email duplicado
                return -1;
            }
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
            var sql = @"DELETE FROM Propietarios WHERE Id = @id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            try
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)
                    return -1;

                throw;
            }
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT Id, Dni, Apellido, Nombre, Telefono, Email
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
                    Telefono = reader.GetString(4),
                    Email = reader.GetString(5)
                };
            }

            return p;
        }
    }
}
