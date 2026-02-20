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

        // =========================
        // ALTA
        // =========================
        public int Alta(Propietario p)
        {
            try
            {
                using var conn = GetConnection();

                var sql = @"
                    INSERT INTO Propietarios 
                    (Dni, Apellido, Nombre, Email, Telefono)
                    VALUES (@dni, @apellido, @nombre, @correo, @telefono);
                    SELECT LAST_INSERT_ID();
                ";

                using var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@dni", p.Dni);
                cmd.Parameters.AddWithValue("@apellido", p.Apellido);
                cmd.Parameters.AddWithValue("@nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@correo", p.Email);
                cmd.Parameters.AddWithValue("@telefono", p.Telefono);

                conn.Open();
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                p.Id = id;

                return id;
            }
            catch (MySqlException)
            {
                return -1; // DNI o Email duplicado
            }
        }

        // =========================
        // OBTENER TODOS
        // =========================
        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();
            using var conn = GetConnection();

            var sql = @"
                SELECT Id, Dni, Apellido, Nombre, Email, Telefono
                FROM Propietarios
            ";

            using var cmd = new MySqlCommand(sql, conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();

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

        // =========================
        // BUSCAR POR ID
        // =========================
        public Propietario? Buscar(int id)
        {
            using var conn = GetConnection();

            var sql = @"
                SELECT Id, Dni, Apellido, Nombre, Email, Telefono
                FROM Propietarios
                WHERE Id = @id
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Propietario
                {
                    Id = reader.GetInt32(0),
                    Dni = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Nombre = reader.GetString(3),
                    Email = reader.GetString(4),
                    Telefono = reader.GetString(5)
                };
            }

            return null;
        }

        // =========================
        // MODIFICACIÓN
        // =========================
        public int Modificacion(Propietario p)
        {
            using var conn = GetConnection();

            var sql = @"
                UPDATE Propietarios SET
                    Dni=@dni,
                    Apellido=@apellido,
                    Nombre=@nombre,
                    Email=@correo,
                    Telefono=@telefono
                WHERE Id=@id
            ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@dni", p.Dni);
            cmd.Parameters.AddWithValue("@apellido", p.Apellido);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@correo", p.Email);
            cmd.Parameters.AddWithValue("@telefono", p.Telefono);
            cmd.Parameters.AddWithValue("@id", p.Id);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // =========================
        // BAJA
        // =========================
        public int Baja(int id)
        {
            try
            {
                using var conn = GetConnection();

                var sql = @"
                    DELETE FROM Propietarios
                    WHERE Id = @id
                ";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)
                    return -1; // Tiene inmuebles asociados

                throw;
            }
        }

    }
}
