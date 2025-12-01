using Microsoft.AspNetCore.Mvc;
using InmobiliariaUlP_2025.Models;

namespace InmobiliariaUlP_2025.Controllers
{
    public class PropietariosController : Controller
    {
        // Repositorio que usa ADO.NET para hablar con MySQL.
        private readonly RepositorioPropietario repositorioPropietario;

        // El framework inyecta el repositorio automáticamente.
        public PropietariosController(RepositorioPropietario repositorioPropietario)
        {
            this.repositorioPropietario = repositorioPropietario;
        }

        // GET: /Propietarios
        public IActionResult Index()
        {
            var lista = repositorioPropietario.ObtenerTodos();
            return View(lista);
        }

        // GET: /Propietarios/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Propietarios/Crear
        [HttpPost]
        public IActionResult Crear(Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            repositorioPropietario.Alta(propietario);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Propietarios/Editar/5
        public IActionResult Editar(int id)
        {
            var propietario = repositorioPropietario.Buscar(id);

            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        // POST: /Propietarios/Editar
        [HttpPost]
        public IActionResult Editar(Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            repositorioPropietario.Modificacion(propietario);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Propietarios/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var propietario = repositorioPropietario.Buscar(id);

            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        // POST: /Propietarios/EliminarConfirmado
        [HttpPost]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorioPropietario.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
