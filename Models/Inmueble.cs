using System.ComponentModel.DataAnnotations;
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

        // =========================
        // RELACIÓN
        // =========================
        [Required(ErrorMessage = "Debe seleccionar un propietario.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un propietario.")]
        public int PropietarioId { get; set; }

        public Propietario? Propietario { get; set; }

        // =========================
        // CAMPOS
        // =========================
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar el uso del inmueble.")]
        public Uso Uso { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de inmueble.")]
        public Tipo Tipo { get; set; }

        [Required(ErrorMessage = "Debe ingresar la cantidad de ambientes.")]
        [Range(1, 20, ErrorMessage = "Los ambientes deben estar entre 1 y 20.")]
        public int NoAmbientes { get; set; }

        // =========================
        // COORDENADAS (VALIDADAS)
        // =========================
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
        public double Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
        public double Longitud { get; set; }

        [Required(ErrorMessage = "Debe ingresar el precio.")]
        [Range(1, 99999999, ErrorMessage = "Debe ingresar un precio válido.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la disponibilidad.")]
        public Disponibilidad Disponibilidad { get; set; }
    }

    // =========================
    // ENUMS
    // =========================
    public enum Uso
    {
        COMERCIAL = 0,
        RESIDENCIAL = 1
    }

    public enum Tipo
    {
        LOCAL = 0,
        DEPOSITO = 1,
        CASA = 2,
        DEPARTAMENTO = 3
    }

    public enum Disponibilidad
    {
        OCUPADO = 0,
        SUSPENDIDO = 1,
        DESOCUPADO = 2
    }
}
