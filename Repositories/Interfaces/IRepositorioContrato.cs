using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioContrato
    {
        // ABM
        int Alta(Contrato contrato);
        int Baja(int id);
        int Modificacion(Contrato contrato);

        // Consultas
        Contrato? ObtenerPorId(int id);
        IList<Contrato> ObtenerTodos();
        IList<Contrato> ObtenerVigentes();
        IList<Contrato> ObtenerPorInmueble(int inmuebleId);

        // Lógica
        bool EstaOcupado(int inmuebleId, DateOnly inicio, DateOnly fin, int? contratoId = null);
        int TerminarContratoAnticipadamente(int id, DateOnly nuevaFechaFin);
        int ObtenerSiguienteNumeroContrato();
    }
}