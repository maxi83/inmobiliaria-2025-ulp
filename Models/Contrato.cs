using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySqlConnector;

namespace InmobiliariaUlP_2025.Models
{
    public class Contrato
    {
        public Contrato() { }

        public Contrato(MySqlDataReader reader)
        {
            Id = reader.GetInt32("Id");
            InmuebleId = reader.GetInt32("InmuebleId");
            InquilinoId = reader.GetInt32("InquilinoId");
            FechaInicio = DateOnly.FromDateTime(reader.GetDateTime("FechaInicio"));
            FechaFin = DateOnly.FromDateTime(reader.GetDateTime("FechaFin"));
            MontoMensual = reader.GetDecimal("MontoMensual");
        }

        [Key]
        public int Id { get; set; }

        // Relaciones
        public int InmuebleId { get; set; }
        public virtual Inmueble? Inmueble { get; set; }

        public int InquilinoId { get; set; }
        public virtual Inquilino? Inquilino { get; set; }

        // Datos del contrato
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public decimal MontoMensual { get; set; }
    }
}
