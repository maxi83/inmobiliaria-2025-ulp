using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioPago
    {
        int Alta(Pago pago);
        int Baja(int id);
        int Modificacion(Pago pago);
        Pago? ObtenerPorId(int id);
        IList<Pago> ObtenerPorContrato(int contratoId);
        int ObtenerSiguienteNumeroPago(int contratoId);
    }
}
