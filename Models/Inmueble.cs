using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaUlP_2025.Models
{
    // =========================================================
    // 💙 CLASE INMUEBLE
    //
    // Representa una propiedad que se alquila.
    // Tiene un Propietario, dirección, precio, tipo, uso, etc.
    //
    // NOTA: Aunque no usemos Entity Framework, los atributos
    // [Key] o [ForeignKey] NO molestan, y son útiles si el profe
    // revisa que esté correctamente modelado.
    // =========================================================
    public class Inmueble
    {
        // Constructor vacío requerido
        public Inmueble() { }

        // ---------- ID ----------
        [Key] // clave primaria
        public int Id { get; set; }

        // ---------- RELACIÓN ----------
        // Id del propietario dueño del inmueble
        [ForeignKey("Propietario")]
        public int PropietarioId { get; set; }

        // Relación 1:N con Propietario (opcional para ADO.NET)
        public virtual Propietario? Propietario { get; set; }

        // ---------- DATOS DEL INMUEBLE ----------
        // Dirección física del inmueble
        public string Direccion { get; set; } = "";

        // Tipo de uso: comercial o residencial
        public Uso Uso { get; set; }

        // Tipo de inmueble
        public Tipo Tipo { get; set; }

        // Cantidad de ambientes
        public int NoAmbientes { get; set; }

        // Ubicación geográfica opcional
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        // Precio actual del alquiler
        public decimal Precio { get; set; }

        // Estado: disponible, ocupado, suspendido
        public Disponibilidad Disponibilidad { get; set; }
    }

    // =========================================================
    //           ENUMERACIONES (listas de valores fijos)
    // =========================================================

    // Cómo se usa el inmueble
    public enum Uso
    {
        COMERCIAL,
        RESIDENCIAL,
    }

    // Qué tipo de inmueble es
    public enum Tipo
    {
        LOCAL,
        DEPOSITO,
        CASA,
        DEPARTAMENTO,
    }

    // Estado actual del inmueble
    public enum Disponibilidad
    {
        OCUPADO,
        SUSPENDIDO,
        DESOCUPADO,
    }
}
