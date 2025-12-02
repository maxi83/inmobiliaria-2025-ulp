using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration)
            : base(configuration)
        {
        }

        // ===========================================
        // LISTAR TODOS
        // ===========================================
        public List<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT i.Id, i.PropietarioId, p.Apellido, p.Nombre, 
                       i.Direccion, i.Uso, i.Tipo, i.NoAmbientes,
                       i.Latitud, i.Longitud, i.Precio, i.Disponibilidad
                FROM Inmuebles i
                INNER JOIN Propietarios p ON p.Id = i.PropietarioId;
            ";

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Inmueble
                {
                    Id = reader.GetInt32(0),
                    PropietarioId = reader.GetInt32(1),
                    Propietario = new Propietario
                    {
                        Id = reader.GetInt32(1),
                        Apellido = reader.GetString(2),
                        Nombre = reader.GetString(3)
                    },
                    Direccion = reader.GetString(4),
                    Uso = (Uso)reader.GetInt32(5),
                    Tipo = (Tipo)reader.GetInt32(6),
                    NoAmbientes = reader.GetInt32(7),
                    Latitud = reader.GetDouble(8),
                    Longitud = reader.GetDouble(9),
                    Precio = reader.GetDecimal(10),
                    Disponibilidad = (Disponibilidad)reader.GetInt32(11)
                });
            }

            return lista;
        }

        // ===========================================
        // BUSCAR POR ID
        // ===========================================
        public Inmueble? Buscar(int id)
        {
            Inmueble? inmueble = null;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT Id, PropietarioId, Direccion, Uso, Tipo,
                       NoAmbientes, Latitud, Longitud, Precio, Disponibilidad
                FROM Inmuebles
                WHERE Id = @id;
            ";

            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                inmueble = new Inmueble
                {
                    Id = reader.GetInt32(0),
                    PropietarioId = reader.GetInt32(1),
                    Direccion = reader.GetString(2),
                    Uso = (Uso)reader.GetInt32(3),
                    Tipo = (Tipo)reader.GetInt32(4),
                    NoAmbientes = reader.GetInt32(5),
                    Latitud = reader.GetDouble(6),
                    Longitud = reader.GetDouble(7),
                    Precio = reader.GetDecimal(8),
                    Disponibilidad = (Disponibilidad)reader.GetInt32(9)
                };
            }

            return inmueble;
        }

        // ===========================================
        // ALTA
        // ===========================================
        public int Alta(Inmueble i)
        {
            int id;

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Inmuebles
                (PropietarioId, Direccion, Uso, Tipo, NoAmbientes, Latitud, Longitud, Precio, Disponibilidad)
                VALUES (@propId, @dir, @uso, @tipo, @amb, @lat, @lon, @precio, @disp);
                SELECT LAST_INSERT_ID();
            ";

            command.Parameters.AddWithValue("@propId", i.PropietarioId);
            command.Parameters.AddWithValue("@dir", i.Direccion);
            command.Parameters.AddWithValue("@uso", i.Uso);
            command.Parameters.AddWithValue("@tipo", i.Tipo);
            command.Parameters.AddWithValue("@amb", i.NoAmbientes);
            command.Parameters.AddWithValue("@lat", i.Latitud);
            command.Parameters.AddWithValue("@lon", i.Longitud);
            command.Parameters.AddWithValue("@precio", i.Precio);
            command.Parameters.AddWithValue("@disp", i.Disponibilidad);

            connection.Open();
            id = System.Convert.ToInt32(command.ExecuteScalar());
            i.Id = id;

            return id;
        }

        // ===========================================
        // MODIFICAR
        // ===========================================
        public int Modificacion(Inmueble i)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Inmuebles SET
                    PropietarioId=@propId,
                    Direccion=@dir,
                    Uso=@uso,
                    Tipo=@tipo,
                    NoAmbientes=@amb,
                    Latitud=@lat,
                    Longitud=@lon,
                    Precio=@precio,
                    Disponibilidad=@disp
                WHERE Id=@id;
            ";

            command.Parameters.AddWithValue("@propId", i.PropietarioId);
            command.Parameters.AddWithValue("@dir", i.Direccion);
            command.Parameters.AddWithValue("@uso", i.Uso);
            command.Parameters.AddWithValue("@tipo", i.Tipo);
            command.Parameters.AddWithValue("@amb", i.NoAmbientes);
            command.Parameters.AddWithValue("@lat", i.Latitud);
            command.Parameters.AddWithValue("@lon", i.Longitud);
            command.Parameters.AddWithValue("@precio", i.Precio);
            command.Parameters.AddWithValue("@disp", i.Disponibilidad);
            command.Parameters.AddWithValue("@id", i.Id);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        // ===========================================
        // BAJA
        // ===========================================
        public int Baja(int id)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"DELETE FROM Inmuebles WHERE Id=@id;";
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        // ===========================================
        // LISTAR PROPIETARIOS (para combos)
        // ===========================================
        public List<Propietario> ObtenerPropietarios()
        {
            var lista = new List<Propietario>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT Id, Apellido, Nombre FROM Propietarios;";

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Propietario
                {
                    Id = reader.GetInt32(0),
                    Apellido = reader.GetString(1),
                    Nombre = reader.GetString(2)
                });
            }

            return lista;
        }
    }
}
