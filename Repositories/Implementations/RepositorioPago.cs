using MySqlConnector;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration config) : base(config) { }

        public int Alta(Pago pago)
        {
            using var conn = GetConnection();
            var sql = @"
                INSERT INTO Pagos (NoPago, FechaPago, Importe, ContratoId)
                VALUES (@noPago, @fecha, @importe, @contratoId);
                SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@noPago", pago.NoPago);
            cmd.Parameters.AddWithValue("@fecha", pago.FechaPago.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@importe", pago.Importe);
            cmd.Parameters.AddWithValue("@contratoId", pago.ContratoId);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // 🔴 ESTE MÉTODO ES EL QUE FALTABA
        public int Modificacion(Pago pago)
        {
            using var conn = GetConnection();
            var sql = @"
                UPDATE Pagos
                SET FechaPago = @fecha,
                    Importe = @importe
                WHERE Id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fecha", pago.FechaPago.ToDateTime(TimeOnly.MinValue));
            cmd.Parameters.AddWithValue("@importe", pago.Importe);
            cmd.Parameters.AddWithValue("@id", pago.Id);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var conn = GetConnection();
            var sql = @"DELETE FROM Pagos WHERE Id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public Pago? ObtenerPorId(int id)
        {
            using var conn = GetConnection();
            var sql = @"SELECT * FROM Pagos WHERE Id = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            return reader.Read() ? new Pago(reader) : null;
        }

        public IList<Pago> ObtenerPorContrato(int contratoId)
        {
            var lista = new List<Pago>();

            using var conn = GetConnection();
            var sql = @"SELECT * FROM Pagos WHERE ContratoId = @id ORDER BY NoPago;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", contratoId);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(new Pago(reader));

            return lista;
        }

        public int ObtenerSiguienteNumeroPago(int contratoId)
        {
            using var conn = GetConnection();
            var sql = @"SELECT IFNULL(MAX(NoPago), 0) + 1 FROM Pagos WHERE ContratoId = @id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", contratoId);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
