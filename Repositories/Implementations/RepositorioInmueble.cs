using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }

        // =========================
        // LISTAR TODOS
        // =========================
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

        // =========================
        // BUSCAR POR ID
        // =========================
        public Inmueble? Buscar(int id)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM Inmuebles WHERE Id=@id;";
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            return reader.Read() ? new Inmueble(reader) : null;
        }

        // =========================
        // ALTA (control UNIQUE Dirección)
        // =========================
        public int Alta(Inmueble i)
        {
            try
            {
                using var connection = GetConnection();
                using var command = connection.CreateCommand();

                command.CommandText = @"
                    INSERT INTO Inmuebles
                    (PropietarioId, Direccion, Uso, Tipo, NoAmbientes, Latitud, Longitud, Precio, Disponibilidad)
                    VALUES (@prop, @dir, @uso, @tipo, @amb, @lat, @lon, @precio, @disp);
                    SELECT LAST_INSERT_ID();
                ";

                command.Parameters.AddWithValue("@prop", i.PropietarioId);
                command.Parameters.AddWithValue("@dir", i.Direccion);
                command.Parameters.AddWithValue("@uso", i.Uso);
                command.Parameters.AddWithValue("@tipo", i.Tipo);
                command.Parameters.AddWithValue("@amb", i.NoAmbientes);
                command.Parameters.AddWithValue("@lat", i.Latitud);
                command.Parameters.AddWithValue("@lon", i.Longitud);
                command.Parameters.AddWithValue("@precio", i.Precio);
                command.Parameters.AddWithValue("@disp", i.Disponibilidad);

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (MySqlException)
            {
                return -1; // Dirección duplicada
            }
        }

        // =========================
        // MODIFICAR
        // =========================
        public int Modificacion(Inmueble i)
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Inmuebles SET
                    PropietarioId=@prop,
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

            command.Parameters.AddWithValue("@prop", i.PropietarioId);
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

        // =========================
        // BAJA
        // =========================
        public int Baja(int id)
        {
            try
            {
                using var connection = GetConnection();
                using var command = connection.CreateCommand();

                command.CommandText = "DELETE FROM Inmuebles WHERE Id=@id;";
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                return command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                return -1; // 👈 NO se pudo borrar (tiene contratos)
            }
        }



        // =========================
        // FILTROS (INTERFAZ)
        // =========================
        public IList<Inmueble> ObtenerPorDisponibilidad(Disponibilidad dispo)
        {
            var lista = new List<Inmueble>();

            using var connection = GetConnection();
            using var command = new MySqlCommand(
                "SELECT * FROM Inmuebles WHERE Disponibilidad=@d", connection);

            command.Parameters.AddWithValue("@d", (int)dispo);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Inmueble(reader));

            return lista;
        }

        public IList<Inmueble> ObtenerPorPropietario(int propietarioId)
        {
            var lista = new List<Inmueble>();

            using var connection = GetConnection();
            using var command = new MySqlCommand(
                "SELECT * FROM Inmuebles WHERE PropietarioId=@id", connection);

            command.Parameters.AddWithValue("@id", propietarioId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Inmueble(reader));

            return lista;
        }

        public List<Propietario> ObtenerPropietarios()
        {
            var lista = new List<Propietario>();

            using var connection = GetConnection();
            using var command = new MySqlCommand(
                "SELECT Id, Apellido, Nombre FROM Propietarios", connection);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Propietario(reader));

            return lista;
        }

        public IList<Inmueble> ObtenerDisponiblesEntreFechas(DateOnly inicio, DateOnly fin)
        {
            var lista = new List<Inmueble>();

            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT *
                FROM Inmuebles i
                WHERE i.Disponibilidad = @desocupado
                AND i.Id NOT IN (
                    SELECT c.InmuebleId
                    FROM Contratos c
                    WHERE c.FechaInicio <= @fin
                        AND c.FechaFin >= @inicio
                );";

            command.Parameters.AddWithValue("@inicio", inicio.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@fin", fin.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@suspendido", (int)Disponibilidad.SUSPENDIDO);
            command.Parameters.AddWithValue("@desocupado", (int)Disponibilidad.DESOCUPADO);


            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Inmueble(reader));

            return lista;
        }


    }
}
