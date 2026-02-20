using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioContrato
    {
        int Alta(Contrato contrato);
        int Baja(int id);
        int Modificacion(Contrato contrato);
        Contrato? ObtenerPorId(int id);
        IList<Contrato> ObtenerTodos();
        IList<Contrato> ObtenerVigentes();
        IList<Contrato> ObtenerPorInmueble(int inmuebleId);
        IList<Contrato> ObtenerPorInquilino(int inquilinoId);
        bool EstaOcupado(int inmuebleId, DateOnly inicio, DateOnly fin, int? excluirId = null);

        // Requeridos por enunciado
        int ObtenerSiguienteNumeroContrato();
         public bool TieneContratosVigentes(int inmuebleId);
        int TerminarContratoAnticipadamente(int id, DateOnly nuevaFechaFin);
        int RenovarContrato(Contrato nuevoContrato, int idContratoAnterior);
    }
}
