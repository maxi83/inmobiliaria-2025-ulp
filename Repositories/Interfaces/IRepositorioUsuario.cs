using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Repositories.Interfaces
{
    public interface IRepositorioUsuario
    {
        List<Usuario> ObtenerTodos();
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        int Alta(Usuario usuario);
        int Modificacion(Usuario usuario);
        int Baja(int id);
    }
}