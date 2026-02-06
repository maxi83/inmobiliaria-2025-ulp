using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize] // 🔒 requiere login
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repositorioInquilino;

        public InquilinosController(IRepositorioInquilino repositorioInquilino)
        {
            this.repositorioInquilino = repositorioInquilino;
        }

        public IActionResult Index()
        {
            var lista = repositorioInquilino.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
                return View(inquilino);

            var resultado = repositorioInquilino.Alta(inquilino);

            if (resultado == -1)
            {
                ViewBag.Error = "Ya existe un inquilino con ese DNI o email.";
                return View(inquilino);
            }

            TempData["Mensaje"] = "Inquilino creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Editar(int id)
        {
            var inquilino = repositorioInquilino.ObtenerPorId(id);
            if (inquilino == null)
                return NotFound();

            return View(inquilino);
        }

        [HttpPost]
        public IActionResult Editar(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
                return View(inquilino);

            repositorioInquilino.Modificacion(inquilino);
            TempData["Mensaje"] = "Inquilino modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // 🔒 SOLO ADMIN
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inquilino = repositorioInquilino.ObtenerPorId(id);
            if (inquilino == null)
                return NotFound();

            return View(inquilino);
        }

        // 🔒 SOLO ADMIN
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var resultado = repositorioInquilino.Baja(id);

            if (resultado == -1)
            {
                TempData["Error"] = "No se puede eliminar el inquilino porque tiene contratos asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Inquilino eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
