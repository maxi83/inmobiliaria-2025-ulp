using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize]
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repoPropietario;

        public PropietariosController(IRepositorioPropietario repoPropietario)
        {
            this.repoPropietario = repoPropietario;
        }

        // =========================
        // LISTADO
        // =========================

        public IActionResult Index()
        {
            return View(repoPropietario.ObtenerTodos());
        }

        // =========================
        // ALTA
        // =========================

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Propietario propietario)
        {
            if (!ModelState.IsValid)
                return View(propietario);

            var resultado = repoPropietario.Alta(propietario);

            // Regla de negocio: evitar DNI o email duplicado
            if (resultado == -1)
            {
                ViewBag.Error = "Ya existe un propietario con ese DNI o email.";
                return View(propietario);
            }

            TempData["Mensaje"] = "Propietario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MODIFICACIÓN
        // =========================

        public IActionResult Editar(int id)
        {
            var propietario = repoPropietario.Buscar(id);
            if (propietario == null) return NotFound();

            return View(propietario);
        }

        [HttpPost]
        public IActionResult Editar(Propietario propietario)
        {
            if (!ModelState.IsValid)
                return View(propietario);

            repoPropietario.Modificacion(propietario);

            TempData["Mensaje"] = "Propietario modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // BAJA (Solo Administrador)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var propietario = repoPropietario.Buscar(id);
            if (propietario == null) return NotFound();

            return View(propietario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var resultado = repoPropietario.Baja(id);

            // Regla de negocio: no permitir eliminar si tiene inmuebles asociados
            if (resultado == -1)
            {
                TempData["Error"] =
                    "No se puede eliminar el propietario porque tiene inmuebles asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Propietario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}