// -----------------------------------------------------------------------------
// using = palabra clave de C# para "importar" espacios de nombres (namespaces).
// Nos permite usar clases que están definidas en otras librerías sin escribir
// el nombre completo todo el tiempo (por ejemplo, MySqlConnection en vez de
// MySql.Data.MySqlClient.MySqlConnection).
// -----------------------------------------------------------------------------

// IConfiguration está en Microsoft.Extensions.Configuration.
// Se usa para leer valores de configuración, como la cadena de conexión
// desde el archivo appsettings.json.
using Microsoft.Extensions.Configuration;

// MySqlConnection, MySqlCommand, etc. están en el namespace MySql.Data.MySqlClient.
// Esto viene del paquete MySql.Data que instalaste con "dotnet add package MySql.Data".
using MySql.Data.MySqlClient;

// System = incluye tipos básicos del sistema, como Exception, Convert, etc.
using System;

// System.Collections.Generic = nos permite usar List<T>, por ejemplo List<Propietario>.
using System.Collections.Generic;

// System.Data = contiene tipos para trabajar con datos, como CommandType.
using System.Data;

// -----------------------------------------------------------------------------
// namespace = "apellido lógico" de las clases.
// Agrupa las clases dentro de un contexto para que no choquen nombres.
// Tiene que coincidir con el namespace general de tu proyecto.
// En tu caso, algo así como InmobiliariaUlP_2025.Models está perfecto.
// -----------------------------------------------------------------------------
namespace InmobiliariaUlP_2025.Models
{
    // -------------------------------------------------------------------------
    // CLASE RepositorioPropietario
    //
    // Esta clase se encarga del ACCESO A DATOS (ADO.NET) para la entidad
    // Propietario. Es decir:
    //  - Conectarse a la base de datos MySQL.
    //  - Ejecutar INSERT, SELECT, UPDATE, DELETE sobre la tabla Propietarios.
    //  - Transformar los datos de la base en objetos Propietario y viceversa.
    //
    // Aplica el patrón REPOSITORIO: separa la lógica de acceso a datos
    // del resto de la aplicación (controladores, vistas, etc.).
    // -------------------------------------------------------------------------
    public class RepositorioPropietario
    {
        // ---------------------------------------------------------------------
        // CAMPOS PRIVADOS
        // ---------------------------------------------------------------------

        // "private readonly IConfiguration configuration;"
        // - private: solo se puede usar dentro de esta clase.
        // - readonly: solo se puede asignar en el constructor.
        // - IConfiguration: interfaz para leer configuración.
        // Se guarda una referencia a la configuración de la app
        // (para acceder a appsettings.json).
        private readonly IConfiguration configuration;

        // "private readonly string connectionString;"
        // - string connectionString: texto que contiene la cadena de conexión
        //   a la base de datos MySQL (servidor, base, usuario, contraseña, etc.).
        // La leemos una vez en el constructor y la usamos en todos los métodos.
        private readonly string connectionString;

        // ---------------------------------------------------------------------
        // CONSTRUCTOR
        // ---------------------------------------------------------------------

        // El constructor recibe IConfiguration desde afuera (por inyección de dependencias).
        // Esto lo va a hacer el framework automáticamente cuando creemos el repositorio
        // en un controlador.
        public RepositorioPropietario(IConfiguration configuration)
        {
            // "this.configuration = configuration;"
            // this.configuration → el campo privado de la clase.
            // configuration (parámetro) → lo que llega desde afuera.
            // Asignamos el parámetro al campo para poder usarlo después.
            this.configuration = configuration;

            // "configuration["ConnectionStrings:DefaultConnection"]"
            // Busca en appsettings.json dentro de:
            // "ConnectionStrings": {
            //    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;"
            // }
            // Si no existe, devolvemos null y lanzamos una excepción.
            connectionString = configuration["ConnectionStrings:DefaultConnection"]
                               ?? throw new InvalidOperationException(
                                   "Falta ConnectionStrings:DefaultConnection en appsettings.json");
        }

        // ---------------------------------------------------------------------
        // MÉTODO: Alta
        // Inserta un nuevo propietario en la tabla Propietarios.
        // Devuelve el Id (auto_increment) generado por MySQL.
        // ---------------------------------------------------------------------
        public int Alta(Propietario p)
        {
            // "int res = -1;" → inicializamos el resultado con -1 (valor de error por defecto).
            int res = -1;

            // using (MySqlConnection connection = new MySqlConnection(connectionString))
            // Crea un objeto MySqlConnection con la cadena de conexión.
            // El using garantiza que al salir del bloque se llame a Dispose(),
            // lo que cierra la conexión automáticamente.
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // string sql = "..." → definimos el comando SQL a ejecutar.
                // INSERT INTO: inserta una fila nueva.
                // LAST_INSERT_ID(): devuelve el último Id auto_increment generado.
                string sql = @"INSERT INTO Propietarios (Dni, Apellido, Nombre, Email, Telefono) 
                               VALUES (@dni, @apellido, @nombre, @correo, @telefono);
                               SELECT LAST_INSERT_ID();";

                // using (MySqlCommand command = new MySqlCommand(sql, connection))
                // Crea el comando que se va a ejecutar contra la base de datos.
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    // Indicamos que el comando es de tipo texto (SQL directo).
                    command.CommandType = CommandType.Text;

                    // Agregamos parámetros para evitar SQL Injection y
                    // para pasar los valores desde el objeto Propietario.
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@correo", p.Email);     // Email del propietario
                    command.Parameters.AddWithValue("@telefono", p.Telefono); // Teléfono del propietario

                    // Abrimos la conexión a la base de datos.
                    connection.Open();

                    // ExecuteScalar ejecuta el comando y devuelve la primera columna
                    // de la primera fila del resultado. En este caso es LAST_INSERT_ID().
                    var result = command.ExecuteScalar();

                    // Convertimos el resultado a int y lo guardamos en res.
                    res = Convert.ToInt32(result);

                    // Actualizamos el objeto Propietario que recibimos con el Id generado.
                    p.Id = res;

                    // Cerramos la conexión explícitamente (aunque el using también la cerraría).
                    connection.Close();
                }
            }

            // Devolvemos el Id generado.
            return res;
        }

        // ---------------------------------------------------------------------
        // MÉTODO: ObtenerTodos
        // Devuelve una lista con TODOS los propietarios de la tabla Propietarios.
        // ---------------------------------------------------------------------
        public List<Propietario> ObtenerTodos()
        {
            // Creamos la lista que vamos a devolver.
            var listaPropietarios = new List<Propietario>();

            // Abrimos la conexión a MySQL dentro de un using para que se cierre sola.
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // Consulta SELECT para traer todas las columnas que necesitamos.
                string sql = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono 
                               FROM Propietarios;";

                // Creamos el comando con la consulta y la conexión.
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Abrimos la conexión.
                    connection.Open();

                    // ExecuteReader devuelve un cursor (MySqlDataReader)
                    // para leer fila por fila el resultado.
                    var cursor = command.ExecuteReader();

                    // while (cursor.Read()) → mientras haya filas para leer...
                    while (cursor.Read())
                    {
                        // Creamos un nuevo objeto Propietario y mapeamos
                        // cada columna a una propiedad.
                        // GetInt32(0) → primera columna (Id).
                        // GetString(1) → segunda columna (Dni).
                        listaPropietarios.Add(new Propietario
                        {
                            Id = cursor.GetInt32(0),
                            Dni = cursor.GetString(1),
                            Apellido = cursor.GetString(2),
                            Nombre = cursor.GetString(3),
                            Email = cursor.GetString(4),
                            Telefono = cursor.GetString(5),
                        });
                    }

                    // Cerramos la conexión.
                    connection.Close();
                }
            }

            // Devolvemos la lista completa de propietarios.
            return listaPropietarios;
        }

        // ---------------------------------------------------------------------
        // MÉTODO: Baja
        // Elimina un propietario de la tabla según su Id (DELETE).
        // Devuelve la cantidad de filas afectadas (1 si borró, 0 si no encontró).
        // ---------------------------------------------------------------------
        public int Baja(int id)
        {
            int res = -1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // DELETE: borra filas de la tabla Propietarios donde el Id coincida.
                string sql = @"DELETE FROM Propietarios 
                               WHERE Id = @id;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Parámetro @id con el valor que recibimos en el método.
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    // ExecuteNonQuery ejecuta el comando y devuelve cuántas filas
                    // fueron afectadas (en DELETE, UPDATE, INSERT sin SELECT).
                    res = command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            return res;
        }

        // ---------------------------------------------------------------------
        // MÉTODO: Modificacion
        // Actualiza los datos de un propietario existente (UPDATE).
        // Devuelve la cantidad de filas afectadas.
        // ---------------------------------------------------------------------
        public int Modificacion(Propietario p)
        {
            int res = -1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // UPDATE: modifica columnas de la fila donde Id = @id.
                string sql = @"UPDATE Propietarios 
                               SET Dni = @dni, 
                                   Apellido = @apellido, 
                                   Nombre = @nombre, 
                                   Email = @correo,
                                   Telefono = @telefono
                               WHERE Id = @id;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Cargamos los parámetros con los valores del objeto Propietario.
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@correo", p.Email);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@id", p.Id);

                    connection.Open();

                    // ExecuteNonQuery devuelve cuántas filas se actualizaron.
                    res = command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            return res;
        }

        // ---------------------------------------------------------------------
        // MÉTODO: Buscar
        // Busca un propietario por Id y devuelve el objeto Propietario,
        // o null si no existe.
        // ---------------------------------------------------------------------
        public Propietario? Buscar(int id)
        {
            // Inicialmente null: si no encontramos nada, devolvemos null.
            Propietario? p = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // SELECT para traer una sola fila según el Id.
                string sql = @"SELECT Id, Dni, Apellido, Nombre, Email, Telefono 
                               FROM Propietarios 
                               WHERE Id = @id;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    // Agregamos el parámetro @id.
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    // ExecuteReader para leer el resultado.
                    var cursor = command.ExecuteReader();

                    // Si hay una fila, la leemos y mapeamos al objeto Propietario.
                    while (cursor.Read())
                    {
                        p = new Propietario
                        {
                            Id = cursor.GetInt32(0),
                            Dni = cursor.GetString(1),
                            Apellido = cursor.GetString(2),
                            Nombre = cursor.GetString(3),
                            Email = cursor.GetString(4),
                            Telefono = cursor.GetString(5)
                        };
                    }

                    connection.Close();
                }
            }

            // Devolvemos el propietario encontrado (o null si no hubo filas).
            return p;
        }
    }
}
