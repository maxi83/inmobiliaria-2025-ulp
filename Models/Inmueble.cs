using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Models
{
    public class Inmueble
    {
        public Inmueble() { }

        public Inmueble(MySqlDataReader reader)
        {
            Id = reader.GetInt32("Id");
            PropietarioId = reader.GetInt32("PropietarioId");
            Direccion = reader.GetString("Direccion");
            Uso = (Uso)reader.GetInt32("Uso");
            Tipo = (Tipo)reader.GetInt32("Tipo");
            NoAmbientes = reader.GetInt32("NoAmbientes");
            Latitud = reader.GetDouble("Latitud");
            Longitud = reader.GetDouble("Longitud");
            Precio = reader.GetDecimal("Precio");
            Disponibilidad = (Disponibilidad)reader.GetInt32("Disponibilidad");
        }

        [Key]
        public int Id { get; set; }

        // ---- RELACIÓN ----
        [Required(ErrorMessage = "Debe seleccionar un propietario.")]
        public int PropietarioId { get; set; }

        public virtual Propietario? Propietario { get; set; }

        // ---- CAMPOS ----
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
        public string Direccion { get; set; } = "";

        [Required(ErrorMessage = "Debe elegir el uso del inmueble.")]
        public Uso Uso { get; set; }

        [Required(ErrorMessage = "Debe elegir el tipo de inmueble.")]
        public Tipo Tipo { get; set; }

        [Range(1, 20, ErrorMessage = "Los ambientes deben estar entre 1 y 20.")]
        public int NoAmbientes { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }

        [Range(1, 9999999, ErrorMessage = "Debe ingresar un precio válido.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la disponibilidad.")]
        public Disponibilidad Disponibilidad { get; set; }
    }

    // ---- ENUMS ----
    public enum Uso
    {
        COMERCIAL,
        RESIDENCIAL,
    }

    public enum Tipo
    {
        LOCAL,
        DEPOSITO,
        CASA,
        DEPARTAMENTO,
    }

    public enum Disponibilidad
    {
        OCUPADO,
        SUSPENDIDO,
        DESOCUPADO,
    }
}
