// ---------------------------------------------
// using = "voy a usar tipos que están definidos en otros espacios de nombres".
// IConfiguration = para leer configuración (como la connection string) desde appsettings.json.
using Microsoft.Extensions.Configuration;

// MySqlConnection está en el paquete MySql.Data que instalaste recién.
using MySql.Data.MySqlClient;

namespace InmobiliariaUlP_2025.Repositories
{
    // 🔹 CLASE ABSTRACTA REPOSITORIOBASE
    //
    // "abstract" = no se puede crear un objeto directamente de esta clase.
    // Solo sirve como base (padre) para otros repositorios:
    //  - RepositorioPropietario : RepositorioBase
    //  - RepositorioInquilino : RepositorioBase
    //  etc.
    //
    // La idea es que acá centralizamos:
    //  - cómo obtenemos la cadena de conexión
    //  - cómo creamos conexiones MySqlConnection.
    public abstract class RepositorioBase
    {
        // protected = visible en esta clase y en las que heredan de ella.
        // string = texto.
        // connectionString = guarda la cadena de conexión a MySQL que leemos de appsettings.json.
        protected readonly string connectionString;

        // Constructor de RepositorioBase.
        // Se ejecuta cuando creamos un repositorio hijo (por ejemplo, RepositorioPropietario),
        // y recibe IConfiguration para poder leer la configuración de la aplicación.
        protected RepositorioBase(IConfiguration configuration)
        {
            // configuration.GetConnectionString("DefaultConnection")
            // busca dentro de appsettings.json la sección:
            // "ConnectionStrings": { "DefaultConnection": "..." }
            //
            // Si no la encuentra, lanzamos una excepción para que el error sea visible.
            connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Falta la cadena de conexión 'DefaultConnection' en appsettings.json");
        }

        // Método protegido que crea y devuelve una nueva conexión MySQL.
        // Las clases hijas (los repositorios concretos) van a usar este método
        // para conectarse a la base de datos.
        protected MySqlConnection GetConnection()
        {
            // Crea un nuevo objeto MySqlConnection con la connectionString que leímos en el constructor.
            return new MySqlConnection(connectionString);
        }
    }
}
