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
        public int NumeroContrato { get; set; }

        // =========================
        // RELACIONES (ESTO FALTABA)
        // =========================
        public int InmuebleId { get; set; }
        public int InquilinoId { get; set; }

        public Inmueble? Inmueble { get; set; }
        public Inquilino? Inquilino { get; set; }

        // =========================
        // FECHAS
        // =========================
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public DateOnly? FechaFinOriginal { get; set; }

        // =========================
        // MONTO
        // =========================
        public decimal MontoMensual { get; set; }
    }
}
