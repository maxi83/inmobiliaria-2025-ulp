using MySql.Data.MySqlClient;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Repositories.Implementations
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly string connectionString;

        public RepositorioUsuario(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("ConnectionString no configurada");
        }

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();

            using (var con = new MySqlConnection(connectionString))
            {
                string sql = "SELECT * FROM usuarios";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Email = reader.GetString("Email"),
                            Password = reader.GetString("Password"),
                            Rol = reader.GetString("Rol"),
                            Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) 
                                    ? null 
                                    : reader.GetString("Foto")
                        });
                    }
                }
            }

            return lista;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;

            using (var con = new MySqlConnection(connectionString))
            {
                string sql = "SELECT * FROM usuarios WHERE Id = @id";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Email = reader.GetString("Email"),
                            Password = reader.GetString("Password"),
                            Rol = reader.GetString("Rol"),
                            Foto = reader.IsDBNull(reader.GetOrdinal("Foto"))
                                    ? null
                                    : reader.GetString("Foto")
                        };
                    }
                }
            }

            return usuario;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;

            using (var con = new MySqlConnection(connectionString))
            {
                string sql = "SELECT * FROM usuarios WHERE Email = @email";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    con.Open();
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Email = reader.GetString("Email"),
                            Password = reader.GetString("Password"),
                            Rol = reader.GetString("Rol"),
                            Foto = reader.IsDBNull(reader.GetOrdinal("Foto"))
                                    ? null
                                    : reader.GetString("Foto")
                        };
                    }
                }
            }

            return usuario;
        }

        public int Alta(Usuario usuario)
        {
            using (var con = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO usuarios (Email, Password, Rol, Foto)
                               VALUES (@email, @password, @rol, @foto)";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@email", usuario.Email);
                    cmd.Parameters.AddWithValue("@password", usuario.Password);
                    cmd.Parameters.AddWithValue("@rol", usuario.Rol);
                    cmd.Parameters.AddWithValue("@foto", usuario.Foto);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Usuario usuario)
        {
            using (var con = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE usuarios 
                               SET Email = @email,
                                   Password = @password,
                                   Rol = @rol,
                                   Foto = @foto
                               WHERE Id = @id";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@email", usuario.Email);
                    cmd.Parameters.AddWithValue("@password", usuario.Password);
                    cmd.Parameters.AddWithValue("@rol", usuario.Rol);
                    cmd.Parameters.AddWithValue("@foto", usuario.Foto);
                    cmd.Parameters.AddWithValue("@id", usuario.Id);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int Baja(int id)
        {
            using (var con = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM usuarios WHERE Id = @id";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}