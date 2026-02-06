using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize] // 🔒 requiere estar logueado
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

            var resultado = repositorioPropietario.Alta(propietario);

            if (resultado == -1)
            {
                ViewBag.Error = "Ya existe un propietario con ese DNI o email.";
                return View(propietario);
            }

            TempData["Mensaje"] = "Propietario creado correctamente.";
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
            TempData["Mensaje"] = "Propietario modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // 🔒 SOLO ADMIN PUEDE ELIMINAR
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var propietario = repositorioPropietario.Buscar(id);
            if (propietario == null)
                return NotFound();

            return View(propietario);
        }

        // 🔒 SOLO ADMIN PUEDE CONFIRMAR ELIMINACIÓN
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var resultado = repositorioPropietario.Baja(id);

            if (resultado == -1)
            {
                TempData["Error"] = "No se puede eliminar el propietario porque tiene inmuebles asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Propietario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
