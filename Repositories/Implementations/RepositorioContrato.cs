using MySqlConnector;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Repositories
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Contrato contrato)
        {
            using var connection = GetConnection();
            var sql = @"INSERT INTO Contratos (InmuebleId, InquilinoId, FechaInicio, FechaFin, MontoMensual)
                        VALUES (@InmuebleId, @InquilinoId, @FechaInicio, @FechaFin, @MontoMensual);
                        SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
            command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);

            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public int Baja(int id)
        {
            using var connection = GetConnection();
            var sql = @"DELETE FROM Contratos WHERE Id = @Id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Modificacion(Contrato contrato)
        {
            using var connection = GetConnection();
            var sql = @"UPDATE Contratos SET
                        InmuebleId = @InmuebleId,
                        InquilinoId = @InquilinoId,
                        FechaInicio = @FechaInicio,
                        FechaFin = @FechaFin,
                        MontoMensual = @MontoMensual
                        WHERE Id = @Id;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", contrato.Id);
            command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
            command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public Contrato? ObtenerPorId(int id)
        {
            using var connection = GetConnection();
            var sql = @"SELECT * FROM Contratos WHERE Id = @Id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? new Contrato(reader) : null;
        }

        public IList<Contrato> ObtenerTodos()
        {
            var lista = new List<Contrato>();
            using var connection = GetConnection();
            var sql = @"SELECT * FROM Contratos;";

            using var command = new MySqlCommand(sql, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
                lista.Add(new Contrato(reader));

            return lista;
        }

        public IList<Contrato> ObtenerVigentes()
        {
            var lista = new List<Contrato>();
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            using var connection = GetConnection();
            var sql = @"SELECT * FROM Contratos WHERE FechaInicio <= @Hoy AND FechaFin >= @Hoy;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Hoy", hoy.ToDateTime(TimeOnly.MinValue));

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Contrato(reader));

            return lista;
        }

        public IList<Contrato> ObtenerPorInmueble(int inmuebleId)
        {
            var lista = new List<Contrato>();

            using var connection = GetConnection();
            var sql = @"SELECT * FROM Contratos WHERE InmuebleId = @Id;";
            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", inmuebleId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Contrato(reader));

            return lista;
        }

        public IList<Contrato> ObtenerPorInquilino(int inquilinoId)
        {
            var lista = new List<Contrato>();

            using var connection = GetConnection();
            var sql = @"SELECT * FROM Contratos WHERE InquilinoId = @Id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", inquilinoId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Contrato(reader));

            return lista;
        }

        public bool EstaOcupado(int inmuebleId, DateOnly inicio, DateOnly fin, int? excluirId = null)
        {
            using var connection = GetConnection();

            var sql = @"
                SELECT COUNT(*)
                FROM Contratos
                WHERE InmuebleId = @InmuebleId
                AND NOT (FechaFin < @Inicio OR FechaInicio > @Fin)
            ";

            if (excluirId.HasValue)
                sql += " AND Id <> @ExId;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@InmuebleId", inmuebleId);
            command.Parameters.AddWithValue("@Inicio", inicio.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@Fin", fin.ToDateTime(TimeOnly.MinValue));

            if (excluirId.HasValue)
                command.Parameters.AddWithValue("@ExId", excluirId.Value);

            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }
}
