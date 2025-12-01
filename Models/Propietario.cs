using System.ComponentModel.DataAnnotations;          // Por si más adelante queremos validar algo propio de Propietario.
                                                      // (Ahora mismo no usamos DataAnnotations acá, pero no molesta tenerlo).

namespace InmobiliariaUlP_2025.Models                  // 👈 Usá el mismo namespace que en DatosPersonales.cs
{
    // 🧍‍♂️ CLASE PROPIETARIO
    // Esta clase representa al dueño de uno o varios inmuebles.
    //
    // "Propietario : DatosPersonales" significa:
    //  - Propietario HEREDA de DatosPersonales.
    //  - Propietario YA TIENE:
    //      Dni
    //      Nombre
    //      Apellido
    //      Email
    //      Telefono
    //      NombreCompleto
    //
    // Por eso, ACÁ NO volvemos a declarar esas propiedades.
    public class Propietario : DatosPersonales
    {
        // 🔹 Id
        // Esta propiedad es la clave primaria (Primary Key) del propietario en la base de datos.
        // Sirve para identificar de forma única a cada propietario.
        // Ejemplos: 1, 2, 3, 4...
        public int Id { get; set; }

        // 🔹 (A futuro)
        // Cuando creemos la clase Inmueble, podríamos agregar:
        //
        // public List<Inmueble> Inmuebles { get; set; } = new();
        //
        // para representar que:
        //  - Un propietario puede tener varios inmuebles.
        //
        // Por ahora no lo agregamos para mantener el código simple
        // y evitar errores de referencia circular mientras no exista la clase Inmueble.
    }
}
