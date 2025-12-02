using Microsoft.AspNetCore.Mvc;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repositorioPropietario;

        public PropietariosController(IRepositorioPropietario repositorioPropietario)
        {
                 this.repositorioPropietario = repositorioPropietario;
        }


        public IActionResult Index()
        {
            var lista = repositorioPropietario.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Propietario propietario)
        {
            if (!ModelState.IsValid)
                return View(propietario);

            repositorioPropietario.Alta(propietario);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Editar(int id)
        {
            var propietario = repositorioPropietario.Buscar(id);

            if (propietario == null)
                return NotFound();

            return View(propietario);
        }

        [HttpPost]
        public IActionResult Editar(Propietario propietario)
        {
            if (!ModelState.IsValid)
                return View(propietario);

            repositorioPropietario.Modificacion(propietario);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Eliminar(int id)
        {
            var propietario = repositorioPropietario.Buscar(id);

            if (propietario == null)
                return NotFound();

            return View(propietario);
        }

        [HttpPost]
        public IActionResult EliminarConfirmado(int id)
        {
            repositorioPropietario.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
