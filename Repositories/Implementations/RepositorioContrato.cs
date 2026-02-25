using MySqlConnector;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioContrato : IRepositorioContrato
    {
        private readonly string connectionString;

        public RepositorioContrato(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("ConnectionString no configurada");
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // =========================
        // LISTADOS
        // =========================

        public IList<Contrato> ObtenerTodos()
        {
            var res = new List<Contrato>();
            using var conn = GetConnection();

            var sql = @"
                SELECT 
                    c.Id,
                    c.NumeroContrato,
                    c.InmuebleId,
                    c.InquilinoId,
                    c.FechaInicio,
                    c.FechaFin,
                    c.MontoMensual,
                    c.FechaFinOriginal,
                    i.Nombre AS InqNombre,
                    i.Apellido AS InqApellido,
                    inm.Direccion
                FROM Contratos c
                INNER JOIN Inquilinos i ON i.Id = c.InquilinoId
                INNER JOIN Inmuebles inm ON inm.Id = c.InmuebleId
            ";

            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var contrato = new Contrato
                {
                    Id = reader.GetInt32("Id"),
                    NumeroContrato = reader.GetInt32("NumeroContrato"),
                    InmuebleId = reader.GetInt32("InmuebleId"),
                    InquilinoId = reader.GetInt32("InquilinoId"),
                    FechaInicio = DateOnly.FromDateTime(reader.GetDateTime("FechaInicio")),
                    FechaFin = DateOnly.FromDateTime(reader.GetDateTime("FechaFin")),
                    MontoMensual = reader.GetDecimal("MontoMensual"),
                    FechaFinOriginal = reader.IsDBNull(reader.GetOrdinal("FechaFinOriginal"))
                        ? null
                        : DateOnly.FromDateTime(reader.GetDateTime("FechaFinOriginal")),
                    Inquilino = new Inquilino
                    {
                        Nombre = reader.GetString("InqNombre"),
                        Apellido = reader.GetString("InqApellido")
                    },
                    Inmueble = new Inmueble
                    {
                        Direccion = reader.GetString("Direccion")
                    }
                };

                res.Add(contrato);
            }

            return res;
        }

        public IList<Contrato> ObtenerVigentes()
        {
            var res = new List<Contrato>();
            using var conn = GetConnection();

            var sql = @"
                SELECT 
                    c.*,
                    i.Id AS InqId, i.Nombre AS InqNombre, i.Apellido AS InqApellido,
                    inm.Id AS InmId, inm.Direccion
                FROM Contratos c
                INNER JOIN Inquilinos i ON c.InquilinoId = i.Id
                INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id
                WHERE c.FechaInicio <= CURDATE()
                AND c.FechaFin >= CURDATE()
            ";

            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var contrato = new Contrato(reader);

                contrato.Inquilino = new Inquilino
                {
                    Id = reader.GetInt32("InqId"),
                    Nombre = reader.GetString("InqNombre"),
                    Apellido = reader.GetString("InqApellido")
                };

                contrato.Inmueble = new Inmueble
                {
                    Id = reader.GetInt32("InmId"),
                    Direccion = reader.GetString("Direccion")
                };

                res.Add(contrato);
            }

            return res;
        }

        public IList<Contrato> ObtenerPorInmueble(int inmuebleId)
        {
            var res = new List<Contrato>();
            using var conn = GetConnection();

            var sql = @"
                SELECT 
                    c.*,
                    i.Id AS InqId, i.Nombre AS InqNombre, i.Apellido AS InqApellido,
                    inm.Id AS InmId, inm.Direccion
                FROM Contratos c
                INNER JOIN Inquilinos i ON c.InquilinoId = i.Id
                INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id
                WHERE c.InmuebleId = @id
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", inmuebleId);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var contrato = new Contrato(reader);

                contrato.Inquilino = new Inquilino
                {
                    Id = reader.GetInt32("InqId"),
                    Nombre = reader.GetString("InqNombre"),
                    Apellido = reader.GetString("InqApellido")
                };

                contrato.Inmueble = new Inmueble
                {
                    Id = reader.GetInt32("InmId"),
                    Direccion = reader.GetString("Direccion")
                };

                res.Add(contrato);
            }

            return res;
        }

        public Contrato? ObtenerPorId(int id)
        {
            using var conn = GetConnection();

            var sql = @"
                SELECT 
                    c.*,
                    i.Id AS InqId, i.Nombre AS InqNombre, i.Apellido AS InqApellido,
                    inm.Id AS InmId, inm.Direccion
                FROM Contratos c
                INNER JOIN Inquilinos i ON c.InquilinoId = i.Id
                INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id
                WHERE c.Id = @id
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            var contrato = new Contrato(reader);

            contrato.Inquilino = new Inquilino
            {
                Id = reader.GetInt32("InqId"),
                Nombre = reader.GetString("InqNombre"),
                Apellido = reader.GetString("InqApellido")
            };

            contrato.Inmueble = new Inmueble
            {
                Id = reader.GetInt32("InmId"),
                Direccion = reader.GetString("Direccion")
            };

            return contrato;
        }

        public int Alta(Contrato contrato)
        {
            using var conn = GetConnection();

            var sql = @"
                INSERT INTO Contratos
                (NumeroContrato, InmuebleId, InquilinoId, FechaInicio, FechaFin, MontoMensual)
                VALUES (@num,@inm,@inq,@ini,@fin,@monto)
            ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@num", ObtenerSiguienteNumeroContrato());
            cmd.Parameters.AddWithValue("@inm", contrato.InmuebleId);
            cmd.Parameters.AddWithValue("@inq", contrato.InquilinoId);
            cmd.Parameters.AddWithValue("@ini", contrato.FechaInicio.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@fin", contrato.FechaFin.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@monto", contrato.MontoMensual);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Modificacion(Contrato contrato)
        {
            using var conn = GetConnection();

            var sql = @"
                UPDATE Contratos SET
                InmuebleId=@inm,
                InquilinoId=@inq,
                FechaInicio=@ini,
                FechaFin=@fin,
                MontoMensual=@monto
                WHERE Id=@id
            ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", contrato.Id);
            cmd.Parameters.AddWithValue("@inm", contrato.InmuebleId);
            cmd.Parameters.AddWithValue("@inq", contrato.InquilinoId);
            cmd.Parameters.AddWithValue("@ini", contrato.FechaInicio.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@fin", contrato.FechaFin.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@monto", contrato.MontoMensual);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var conn = GetConnection();

            var sql = @"DELETE FROM Contratos WHERE Id = @id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public bool EstaOcupado(int inmuebleId, DateOnly inicio, DateOnly fin, int? contratoId = null)
        {
            using var conn = GetConnection();

            var sql = @"
                SELECT COUNT(*)
                FROM Contratos
                WHERE InmuebleId=@id
                AND FechaInicio <= @fin
                AND FechaFin >= @ini
            ";

            if (contratoId.HasValue)
                sql += " AND Id <> @cid";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", inmuebleId);
            cmd.Parameters.AddWithValue("@ini", inicio.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@fin", fin.ToDateTime(TimeOnly.MinValue));

            if (contratoId.HasValue)
                cmd.Parameters.AddWithValue("@cid", contratoId.Value);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public int TerminarContratoAnticipadamente(int id, DateOnly nuevaFechaFin)
        {
            using var conn = GetConnection();

            var sql = @"
                UPDATE Contratos
                SET FechaFin=@fin
                WHERE Id=@id
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fin", nuevaFechaFin.ToDateTime(TimeOnly.MinValue));

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int ObtenerSiguienteNumeroContrato()
        {
            using var conn = GetConnection();

            var sql = @"SELECT IFNULL(MAX(NumeroContrato),0) + 1 FROM Contratos";

            using var cmd = new MySqlCommand(sql, conn);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}