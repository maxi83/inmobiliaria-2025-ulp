// ---------------------------------------------
// using = "voy a usar cosas que están definidas en otro espacio de nombres".
// En este caso traemos DataAnnotations para poder usar atributos como:
// [Required], [StringLength], [EmailAddress], etc.
using System.ComponentModel.DataAnnotations;

// ---------------------------------------------
// namespace = como el "apellido lógico" de la clase.
// Podés dejar este tal cual, no es obligatorio que coincida con el nombre del proyecto,
// pero si tu proyecto se llama parecido, mejor.
// Si Visual Studio te sugiere otro namespace cuando creás la clase,
// podés usar ese y solo pegar la parte de la clase.
namespace InmobiliariaUlP_2025.Models
{
    // 🧍‍♀️🧍‍♂️ CLASE DATOSPERSONALES
    // Esta clase representa los datos básicos de una persona:
    // - Dni
    // - Nombre
    // - Apellido
    // - Email
    // - Telefono
    //
    // La vamos a usar como clase base (padre) para:
    // - Propietario
    // - Inquilino
    //
    // Así no repetimos las mismas propiedades en varias clases.
    public class DatosPersonales
    {
        // -----------------------------
        // PROPIEDAD: Dni
        // -----------------------------
        // [Required] = el campo es obligatorio en los formularios.
        [Required]
        // [StringLength(20)] = máximo 20 caracteres de largo.
        [StringLength(20)]
        // string = texto.
        // "= string.Empty" = la propiedad empieza como cadena vacía en vez de null.
        public string Dni { get; set; } = string.Empty;

        // -----------------------------
        // PROPIEDAD: Nombre
        // -----------------------------
        // Nombre de pila de la persona (Ej: "Juan").
        [Required]              // obligatorio
        [StringLength(50)]      // máximo 50 caracteres
        public string Nombre { get; set; } = string.Empty;

        // -----------------------------
        // PROPIEDAD: Apellido
        // -----------------------------
        // Apellido de la persona (Ej: "García").
        [Required]              // obligatorio
        [StringLength(50)]      // máximo 50 caracteres
        public string Apellido { get; set; } = string.Empty;

        // -----------------------------
        // PROPIEDAD: Email
        // -----------------------------
        // Dirección de correo electrónico de la persona.
        [Required]              // obligatorio
        [StringLength(100)]     // máximo 100 caracteres
        [EmailAddress]          // valida que tenga formato de email (con @, etc.)
        public string Email { get; set; } = string.Empty;

        // -----------------------------
        // PROPIEDAD: Telefono
        // -----------------------------
        // Número de teléfono de contacto.
        [Required]              // obligatorio
        [StringLength(20)]      // máximo 20 caracteres
        public string Telefono { get; set; } = string.Empty;

        // -----------------------------
        // PROPIEDAD CALCULADA: NombreCompleto
        // -----------------------------
        // No se guarda en la base de datos, se calcula "al vuelo".
        // Usa Apellido y Nombre para armar un texto útil para mostrar:
        // Ejemplo: si Apellido = "García" y Nombre = "Juan",
        // NombreCompleto devuelve "García, Juan".
        public string NombreCompleto => $"{Apellido}, {Nombre}";
    }
}
