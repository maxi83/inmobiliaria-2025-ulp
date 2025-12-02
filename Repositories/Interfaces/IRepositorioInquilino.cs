using System.Collections.Generic;
using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioInquilino
    {
        List<Inquilino> ObtenerTodos();
        Inquilino? Buscar(int id);
        int Alta(Inquilino i);
        int Modificacion(Inquilino i);
        int Baja(int id);
    }
}
