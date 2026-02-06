using System.ComponentModel.DataAnnotations;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Models
{
    public class Contrato
    {
        public Contrato() { }

        public Contrato(MySqlDataReader reader)
        {
            Id = reader.GetInt32("Id");
            NumeroContrato = reader.GetInt32("NumeroContrato");
            InmuebleId = reader.GetInt32("InmuebleId");
            InquilinoId = reader.GetInt32("InquilinoId");
            FechaInicio = DateOnly.FromDateTime(reader.GetDateTime("FechaInicio"));
            FechaFin = DateOnly.FromDateTime(reader.GetDateTime("FechaFin"));
            MontoMensual = reader.GetDecimal("MontoMensual");

            if (!reader.IsDBNull(reader.GetOrdinal("FechaFinOriginal")))
                FechaFinOriginal = DateOnly.FromDateTime(reader.GetDateTime("FechaFinOriginal"));
        }

        public int Id { get; set; }

        // =========================
        // DATOS DEL CONTRATO
        // =========================
        public int NumeroContrato { get; set; }

        // =========================
        // RELACIONES
        // =========================
        [Required(ErrorMessage = "Debe seleccionar un inmueble.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un inmueble.")]
        public int InmuebleId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un inquilino.")]
        public int InquilinoId { get; set; }

        public Inmueble? Inmueble { get; set; }
        public Inquilino? Inquilino { get; set; }

        // =========================
        // FECHAS
        // =========================
        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        public DateOnly FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        public DateOnly FechaFin { get; set; }

        public DateOnly? FechaFinOriginal { get; set; }

        // =========================
        // MONTO
        // =========================
        [Required(ErrorMessage = "El precio mensual es obligatorio.")]
        [Range(1, 99999999, ErrorMessage = "El precio mensual debe ser mayor a 0.")]
        public decimal MontoMensual { get; set; }
    }
}
