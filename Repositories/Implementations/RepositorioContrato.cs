using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContrato(IConfiguration config) : base(config) { }

        // =========================
        // CONSULTA BASE CON JOIN
        // =========================
        private const string SELECT_JOIN = @"
            SELECT 
                c.*,
                i.Id AS Inmueble_Id, i.Direccion,
                q.Id AS Inquilino_Id, q.Apellido, q.Nombre
            FROM Contratos c
            JOIN Inmuebles i ON c.InmuebleId = i.Id
            JOIN Inquilinos q ON c.InquilinoId = q.Id
        ";

        private Contrato MapContrato(MySqlDataReader reader)
        {
            return new Contrato(reader)
            {
                Inmueble = new Inmueble
                {
                    Id = reader.GetInt32("Inmueble_Id"),
                    Direccion = reader.GetString("Direccion")
                },
                Inquilino = new Inquilino
                {
                    Id = reader.GetInt32("Inquilino_Id"),
                    Apellido = reader.GetString("Apellido"),
                    Nombre = reader.GetString("Nombre")
                }
            };
        }

        // =========================
        // OBTENER TODOS
        // =========================
        public IList<Contrato> ObtenerTodos()
        {
            var lista = new List<Contrato>();
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(SELECT_JOIN, conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(MapContrato(reader));

            return lista;
        }

        // =========================
        // OBTENER POR ID
        // =========================
        public Contrato? ObtenerPorId(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(SELECT_JOIN + " WHERE c.Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapContrato(reader) : null;
        }

        // =========================
        // VIGENTES
        // =========================
        public IList<Contrato> ObtenerVigentes()
        {
            var lista = new List<Contrato>();
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                SELECT_JOIN + " WHERE c.FechaFin >= CURDATE()", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(MapContrato(reader));

            return lista;
        }

        // =========================
        // POR INMUEBLE
        // =========================
        public IList<Contrato> ObtenerPorInmueble(int inmuebleId)
        {
            var lista = new List<Contrato>();
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                SELECT_JOIN + " WHERE c.InmuebleId=@id", conn);
            cmd.Parameters.AddWithValue("@id", inmuebleId);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(MapContrato(reader));

            return lista;
        }

        // =========================
        // POR INQUILINO
        // =========================
        public IList<Contrato> ObtenerPorInquilino(int inquilinoId)
        {
            var lista = new List<Contrato>();
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                SELECT_JOIN + " WHERE c.InquilinoId=@id", conn);
            cmd.Parameters.AddWithValue("@id", inquilinoId);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(MapContrato(reader));

            return lista;
        }

        // =========================
        // ALTA
        // =========================
        public int Alta(Contrato c)
        {
            using var conn = GetConnection();
            var sql = @"
                INSERT INTO Contratos
                (NumeroContrato, InmuebleId, InquilinoId, FechaInicio, FechaFin, MontoMensual)
                VALUES (@num, @inm, @inq, @fi, @ff, @monto);
                SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@num", c.NumeroContrato);
            cmd.Parameters.AddWithValue("@inm", c.InmuebleId);
            cmd.Parameters.AddWithValue("@inq", c.InquilinoId);
            cmd.Parameters.AddWithValue("@fi", c.FechaInicio.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@ff", c.FechaFin.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@monto", c.MontoMensual);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // =========================
        // MODIFICAR
        // =========================
        public int Modificacion(Contrato c)
        {
            using var conn = GetConnection();
            var sql = @"
                UPDATE Contratos SET
                    InmuebleId=@inm,
                    InquilinoId=@inq,
                    FechaInicio=@fi,
                    FechaFin=@ff,
                    MontoMensual=@monto
                WHERE Id=@id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inm", c.InmuebleId);
            cmd.Parameters.AddWithValue("@inq", c.InquilinoId);
            cmd.Parameters.AddWithValue("@fi", c.FechaInicio.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@ff", c.FechaFin.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@monto", c.MontoMensual);
            cmd.Parameters.AddWithValue("@id", c.Id);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // =========================
        // BAJA
        // =========================
        public int Baja(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "DELETE FROM Contratos WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                return -1;
            }
        }

        // =========================
        // OCUPADO
        // =========================
        public bool EstaOcupado(int inmuebleId, DateOnly inicio, DateOnly fin, int? contratoId = null)
        {
            using var conn = GetConnection();
            var sql = @"
                SELECT COUNT(*) FROM Contratos
                WHERE InmuebleId=@inm
                AND FechaInicio <= @fin
                AND FechaFin >= @inicio";

            if (contratoId.HasValue)
                sql += " AND Id <> @id";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@inm", inmuebleId);
            cmd.Parameters.AddWithValue("@inicio", inicio.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@fin", fin.ToDateTime(TimeOnly.MinValue));

            if (contratoId.HasValue)
                cmd.Parameters.AddWithValue("@id", contratoId.Value);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public int ObtenerSiguienteNumeroContrato()
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "SELECT IFNULL(MAX(NumeroContrato),0)+1 FROM Contratos", conn);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int TerminarContratoAnticipadamente(int id, DateOnly nuevaFechaFin)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "UPDATE Contratos SET FechaFin=@fin WHERE Id=@id", conn);

            cmd.Parameters.AddWithValue("@fin", nuevaFechaFin.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int RenovarContrato(Contrato nuevo, int contratoAnteriorId)
        {
            return Alta(nuevo);
        }
    }
}
