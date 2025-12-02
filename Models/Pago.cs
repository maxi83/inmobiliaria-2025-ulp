using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Models

{
    public class Pago
    {
        public Pago() { }

        public Pago(MySqlDataReader reader)
        {
            Id = reader.GetInt32("Id");
            NoPago = reader.GetInt32("NoPago");
            Fecha = DateOnly.FromDateTime(reader.GetDateTime("Fecha"));
            Importe = reader.GetDecimal("Importe");
            ContratoId = reader.GetInt32("ContratoId");
        }

        [Key]
        public int Id { get; set; }

        public int NoPago { get; set; }

        public DateOnly Fecha { get; set; }

        public decimal Importe { get; set; }

        [ForeignKey("Contrato")]
        public int ContratoId { get; set; }

        public virtual Contrato? Contrato { get; set; }
    }
}
