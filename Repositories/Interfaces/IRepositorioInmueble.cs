using System.Collections.Generic;
using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioInmueble
    {
        List<Inmueble> ObtenerTodos();
        Inmueble? Buscar(int id);
        int Alta(Inmueble inmueble);
        int Modificacion(Inmueble inmueble);
        int Baja(int id);

        IList<Inmueble> ObtenerPorDisponibilidad(Disponibilidad dispo);
        //IList<Inmueble> ObtenerPorPropietario(int propietarioId);

        // 🔎 Filtro final
        IList<Inmueble> ObtenerDisponiblesEntreFechas(DateOnly inicio, DateOnly fin);
    }
}
