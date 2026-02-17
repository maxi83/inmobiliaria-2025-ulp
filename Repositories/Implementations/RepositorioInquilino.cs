using MySqlConnector;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration config) : base(config) { }

        // =========================
        // ALTA
        // =========================
        public int Alta(Inquilino i)
        {
            try
            {
                using var conn = GetConnection();

                var sql = @"
                    INSERT INTO Inquilinos 
                    (Dni, Nombre, Apellido, Telefono, Email)
                    VALUES (@Dni, @Nombre, @Apellido, @Telefono, @Email);
                    SELECT LAST_INSERT_ID();
                ";

                using var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Dni", i.Dni);
                cmd.Parameters.AddWithValue("@Nombre", i.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", i.Apellido);
                cmd.Parameters.AddWithValue("@Telefono", i.Telefono);
                cmd.Parameters.AddWithValue("@Email", i.Email);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (MySqlException)
            {
                return -1; // DNI o Email duplicado
            }
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
                    DELETE FROM Inquilinos
                    WHERE Id = @Id
                ";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                return -1; // Tiene contratos asociados
            }
        }

        // =========================
        // MODIFICACIÓN
        // =========================
        public int Modificacion(Inquilino i)
        {
            using var conn = GetConnection();

            var sql = @"
                UPDATE Inquilinos SET 
                    Dni=@Dni,
                    Nombre=@Nombre,
                    Apellido=@Apellido,
                    Telefono=@Telefono,
                    Email=@Email
                WHERE Id=@Id
            ";

            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Id", i.Id);
            cmd.Parameters.AddWithValue("@Dni", i.Dni);
            cmd.Parameters.AddWithValue("@Nombre", i.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", i.Apellido);
            cmd.Parameters.AddWithValue("@Telefono", i.Telefono);
            cmd.Parameters.AddWithValue("@Email", i.Email);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // =========================
        // OBTENER POR ID
        // =========================
        public Inquilino? ObtenerPorId(int id)
        {
            using var conn = GetConnection();

            var sql = @"
                SELECT *
                FROM Inquilinos
                WHERE Id = @Id
            ";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            return reader.Read() ? new Inquilino(reader) : null;
        }

        // =========================
        // OBTENER TODOS
        // =========================
        public IList<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();
            using var conn = GetConnection();

            var sql = @"
                SELECT *
                FROM Inquilinos
            ";

            using var cmd = new MySqlCommand(sql, conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                lista.Add(new Inquilino(reader));

            return lista;
        }
    }
}
