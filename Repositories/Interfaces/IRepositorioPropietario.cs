using System.Collections.Generic;
using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioPropietario
    {
        List<Propietario> ObtenerTodos();
        Propietario? Buscar(int id);
        int Alta(Propietario propietario);
        int Modificacion(Propietario propietario);
        int Baja(int id);
        

    }
}
