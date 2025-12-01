// Esta interfaz define qué operaciones debe ofrecer
// un repositorio de Propietario (contrato).
//
// La implementación concreta (que hablará con MySQL)
// va a ir en otra clase, por ejemplo:
//   PropietarioRepository : RepositorioBase, IPropietarioRepository

using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    // La "I" al principio es una convención de C# para interfaces.
    // IPropietarioRepository = "interfaz de repositorio para Propietario".
    public interface IPropietarioRepository
    {
        // 🔹 Listar todos los propietarios
        // Devuelve una lista con todos los propietarios de la base.
        List<Propietario> ObtenerTodos();

        // 🔹 Buscar un propietario por Id
        // Devuelve un Propietario o null si no existe.
        Propietario? ObtenerPorId(int id);

        // 🔹 Agregar un nuevo propietario
        // Recibe un objeto Propietario (sin Id) y lo inserta en la base.
        // Después, idealmente, la implementación puede devolver el Id generado.
        int Crear(Propietario propietario);

        // 🔹 Actualizar un propietario existente
        // Recibe un Propietario con su Id ya cargado y actualiza sus datos.
        void Actualizar(Propietario propietario);

        // 🔹 Eliminar un propietario
        // Para simplificar, lo hacemos borrado físico (DELETE).
        // Más adelante se podría cambiar por "borrado lógico" (Estado = Inactivo).
        void Eliminar(int id);
    }
}
