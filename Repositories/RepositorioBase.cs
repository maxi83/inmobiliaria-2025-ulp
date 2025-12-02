using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Repositories // ← ESTE ES EL BUENO
{
    public abstract class RepositorioBase
    {
        protected readonly string ConnectionString;

        public RepositorioBase(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection'");
        }

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
