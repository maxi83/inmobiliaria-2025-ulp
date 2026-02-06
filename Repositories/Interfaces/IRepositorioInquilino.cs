using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioInquilino
    {
        int Alta(Inquilino i);
        int Baja(int id);
        int Modificacion(Inquilino i);
        Inquilino? ObtenerPorId(int id);
        IList<Inquilino> ObtenerTodos();
    }
}
