using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono
                                    FROM Inquilinos;";

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Inquilino
                {
                    Id = reader.GetInt32(0),
                    Dni = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Nombre = reader.GetString(3),
                    Email = reader.GetString(4),
                    Telefono = reader.GetString(5),
                });
            }

            return lista;
        }

        public Inquilino? Buscar(int id)
        {
            Inquilino? i = null;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono
                                    FROM Inquilinos
                                    WHERE Id = @id;";

            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                i = new Inquilino
                {
                    Id = reader.GetInt32(0),
                    Dni = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Nombre = reader.GetString(3),
                    Email = reader.GetString(4),
                    Telefono = reader.GetString(5),
                };
            }

            return i;
        }

        public int Alta(Inquilino i)
        {
            int id;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"INSERT INTO Inquilinos 
                                    (Dni, Apellido, Nombre, Email, Telefono)
                                    VALUES (@dni, @apellido, @nombre, @correo, @telefono);
                                    SELECT LAST_INSERT_ID();";

            command.Parameters.AddWithValue("@dni", i.Dni);
            command.Parameters.AddWithValue("@apellido", i.Apellido);
            command.Parameters.AddWithValue("@nombre", i.Nombre);
            command.Parameters.AddWithValue("@correo", i.Email);
            command.Parameters.AddWithValue("@telefono", i.Telefono);

            connection.Open();
            id = Convert.ToInt32(command.ExecuteScalar());
            i.Id = id;

            return id;
        }

        public int Modificacion(Inquilino i)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"UPDATE Inquilinos SET
                                    Dni=@dni, Apellido=@apellido, Nombre=@nombre,
                                    Email=@correo, Telefono=@telefono
                                    WHERE Id=@id;";

            command.Parameters.AddWithValue("@dni", i.Dni);
            command.Parameters.AddWithValue("@apellido", i.Apellido);
            command.Parameters.AddWithValue("@nombre", i.Nombre);
            command.Parameters.AddWithValue("@correo", i.Email);
            command.Parameters.AddWithValue("@telefono", i.Telefono);
            command.Parameters.AddWithValue("@id", i.Id);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"DELETE FROM Inquilinos WHERE Id=@id;";
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            return command.ExecuteNonQuery();
        }
    }
}
