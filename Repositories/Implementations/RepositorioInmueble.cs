using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

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
            using var conn = GetConnection();

            var sql = @"
                SELECT 
                    i.Id, i.PropietarioId,
                    p.Apellido, p.Nombre,
                    i.Direccion, i.Uso, i.Tipo,
                    i.NoAmbientes, i.Latitud, i.Longitud,
                    i.Precio, i.Disponibilidad
                FROM Inmuebles i
                INNER JOIN Propietarios p ON p.Id = i.PropietarioId
            ";

            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();

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
            using var conn = GetConnection();

            var sql = @"
                SELECT *
                FROM Inmuebles
                WHERE Id = @id
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            return reader.Read() ? new Inmueble(reader) : null;
        }

        // =========================
        // ALTA
        // =========================
        public int Alta(Inmueble i)
        {
            try
            {
                using var conn = GetConnection();

                var sql = @"
                    INSERT INTO Inmuebles
                    (PropietarioId, Direccion, Uso, Tipo, NoAmbientes, Latitud, Longitud, Precio, Disponibilidad)
                    VALUES (@prop, @dir, @uso, @tipo, @amb, @lat, @lon, @precio, @disp);
                    SELECT LAST_INSERT_ID();
                ";

                using var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@prop", i.PropietarioId);
                cmd.Parameters.AddWithValue("@dir", i.Direccion);
                cmd.Parameters.AddWithValue("@uso", i.Uso);
                cmd.Parameters.AddWithValue("@tipo", i.Tipo);
                cmd.Parameters.AddWithValue("@amb", i.NoAmbientes);
                cmd.Parameters.AddWithValue("@lat", i.Latitud);
                cmd.Parameters.AddWithValue("@lon", i.Longitud);
                cmd.Parameters.AddWithValue("@precio", i.Precio);
                cmd.Parameters.AddWithValue("@disp", i.Disponibilidad);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (MySqlException)
            {
                return -1;
            }
        }

        // =========================
        // MODIFICAR
        // =========================
        public int Modificacion(Inmueble i)
        {
            using var conn = GetConnection();

            var sql = @"
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
                WHERE Id=@id
            ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@prop", i.PropietarioId);
            cmd.Parameters.AddWithValue("@dir", i.Direccion);
            cmd.Parameters.AddWithValue("@uso", i.Uso);
            cmd.Parameters.AddWithValue("@tipo", i.Tipo);
            cmd.Parameters.AddWithValue("@amb", i.NoAmbientes);
            cmd.Parameters.AddWithValue("@lat", i.Latitud);
            cmd.Parameters.AddWithValue("@lon", i.Longitud);
            cmd.Parameters.AddWithValue("@precio", i.Precio);
            cmd.Parameters.AddWithValue("@disp", i.Disponibilidad);
            cmd.Parameters.AddWithValue("@id", i.Id);

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
                    DELETE FROM Inmuebles
                    WHERE Id=@id
                ";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                return -1;
            }
        }

        // =========================
        // FILTROS
        // =========================
        public IList<Inmueble> ObtenerPorDisponibilidad(Disponibilidad dispo)
        {
            var lista = new List<Inmueble>();
            using var conn = GetConnection();

            var sql = @"
                SELECT *
                FROM Inmuebles
                WHERE Disponibilidad = @d
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@d", (int)dispo);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(new Inmueble(reader));

            return lista;
        }

       /* public IList<Inmueble> ObtenerPorPropietario(int propietarioId)
        {
            var lista = new List<Inmueble>();
            using var conn = GetConnection();

            var sql = @"
                SELECT *
                FROM Inmuebles
                WHERE PropietarioId = @id
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", propietarioId);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(new Inmueble(reader));

            return lista;
        }
        */
        public IList<Inmueble> ObtenerDisponiblesEntreFechas(DateOnly inicio, DateOnly fin)
        {
            var lista = new List<Inmueble>();
            using var conn = GetConnection();

            var sql = @"
                SELECT 
                    i.Id, i.PropietarioId,
                    p.Apellido, p.Nombre,
                    i.Direccion, i.Uso, i.Tipo,
                    i.NoAmbientes, i.Latitud, i.Longitud,
                    i.Precio, i.Disponibilidad
                FROM Inmuebles i
                INNER JOIN Propietarios p ON p.Id = i.PropietarioId
                WHERE i.Disponibilidad <> @suspendido
                AND i.Id NOT IN (
                    SELECT c.InmuebleId
                    FROM Contratos c
                    WHERE c.FechaInicio <= @fin
                    AND c.FechaFin >= @inicio
                )
            ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@inicio", inicio.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@fin", fin.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@suspendido", (int)Disponibilidad.SUSPENDIDO);

            conn.Open();
            using var reader = cmd.ExecuteReader();

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
    }
}
