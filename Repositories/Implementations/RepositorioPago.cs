using MySqlConnector;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Repositories
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago pago)
        {
            using var connection = GetConnection();

            var sql = @"INSERT INTO Pagos (NoPago, Fecha, Importe, ContratoId)
                        VALUES (@NoPago, @Fecha, @Importe, @ContratoId);
                        SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@NoPago", pago.NoPago);
            command.Parameters.AddWithValue("@Fecha", pago.Fecha.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@Importe", pago.Importe);
            command.Parameters.AddWithValue("@ContratoId", pago.ContratoId);

            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public int Baja(int id)
        {
            using var connection = GetConnection();
            var sql = @"DELETE FROM Pagos WHERE Id = @Id;";
            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Modificacion(Pago pago)
        {
            using var connection = GetConnection();
            var sql = @"UPDATE Pagos SET
                        NoPago = @NoPago,
                        Fecha = @Fecha,
                        Importe = @Importe
                        WHERE Id = @Id;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", pago.Id);
            command.Parameters.AddWithValue("@NoPago", pago.NoPago);
            command.Parameters.AddWithValue("@Fecha", pago.Fecha.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@Importe", pago.Importe);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public Pago? ObtenerPorId(int id)
        {
            using var connection = GetConnection();
            var sql = @"SELECT * FROM Pagos WHERE Id = @Id;";
            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            return reader.Read() ? new Pago(reader) : null;
        }

        public IList<Pago> ObtenerPorContrato(int contratoId)
        {
            var lista = new List<Pago>();

            using var connection = GetConnection();
            var sql = @"SELECT * FROM Pagos WHERE ContratoId = @ContratoId ORDER BY NoPago ASC;";
            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@ContratoId", contratoId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
                lista.Add(new Pago(reader));

            return lista;
        }

        public int ObtenerSiguienteNumeroPago(int contratoId)
        {
            using var connection = GetConnection();
            var sql = @"SELECT MAX(NoPago) FROM Pagos WHERE ContratoId = @ContratoId;";
            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@ContratoId", contratoId);

            connection.Open();
            var result = command.ExecuteScalar();

            if (result == DBNull.Value)
                return 1;

            return Convert.ToInt32(result) + 1;
        }
    }
}
