using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Models
{
    public class Pago
    {
        public Pago()
        {
            FechaPago = DateOnly.FromDateTime(DateTime.Today);
        }

        public Pago(MySqlDataReader reader)
        {
            Id = reader.GetInt32("Id");
            NoPago = reader.GetInt32("NoPago");
            FechaPago = DateOnly.FromDateTime(reader.GetDateTime("FechaPago"));
            Importe = reader.GetDecimal("Importe");
            ContratoId = reader.GetInt32("ContratoId");
        }

        [Key]
        public int Id { get; set; }

        public int NoPago { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
        [Display(Name = "Fecha de pago")]
        public DateOnly FechaPago { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio.")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "El importe debe ser un número válido.")]
        [Range(1, 1000000000, ErrorMessage = "El importe debe ser mayor a 0 y menor o igual a 1.000.000.000.")]
        public decimal Importe { get; set; }


        [ForeignKey("Contrato")]
        public int ContratoId { get; set; }

        public virtual Contrato? Contrato { get; set; }

        // 🔒 VALIDACIÓN CLAVE
        public static ValidationResult? ValidarFechaPago(DateOnly fecha, ValidationContext context)
        {
            if (fecha > DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidationResult("La fecha de pago no puede ser futura.");
            }

            return ValidationResult.Success;
        }
    }
}
