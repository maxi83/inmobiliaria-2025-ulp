// -----------------------------------------------
// using = habilita el uso de tipos definidos
// en los espacios de nombres que incluimos.
// -----------------------------------------------
using System.Collections.Generic;
using InmobiliariaUlP_2025.Models;
namespace InmobiliariaUlP_2025.Repositories.Interfaces

{
    // -------------------------------------------------------
    // 🔹 INTERFAZ DEL REPOSITORIO DE INQUILINOS
    //
    // Una interfaz define *qué métodos debe tener un repositorio*,
    // pero NO define cómo están implementados.
    //
    // Ventajas:
    //   ✔ Permite inyección de dependencias.
    //   ✔ Aísla al controlador de los detalles de ADO.NET.
    //   ✔ Hace que el proyecto sea más profesional.
    // -------------------------------------------------------
    public interface IRepositorioInquilino
    {
        // Obtener la lista completa de inquilinos
        List<Inquilino> ObtenerTodos();

        // Buscar un inquilino por su ID
        Inquilino? Buscar(int id);

        // Dar de alta un inquilino en la BD
        int Alta(Inquilino i);

        // Modificar un registro existente
        int Modificacion(Inquilino i);

        // Eliminar un registro por ID
        int Baja(int id);
    }
}
