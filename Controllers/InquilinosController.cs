using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
    [Authorize]
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repoInquilino;

        public InquilinosController(IRepositorioInquilino repoInquilino)
        {
            this.repoInquilino = repoInquilino;
        }

        // =========================
        // LISTADO
        // =========================

        public IActionResult Index()
        {
            return View(repoInquilino.ObtenerTodos());
        }

        // =========================
        // ALTA
        // =========================

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
                return View(inquilino);

            var resultado = repoInquilino.Alta(inquilino);

            // Regla de negocio: evitar DNI o email duplicado
            if (resultado == -1)
            {
                ViewBag.Error = "Ya existe un inquilino con ese DNI o email.";
                return View(inquilino);
            }

            TempData["Mensaje"] = "Inquilino creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MODIFICACIÓN
        // =========================

        public IActionResult Editar(int id)
        {
            var inquilino = repoInquilino.ObtenerPorId(id);
            if (inquilino == null) return NotFound();

            return View(inquilino);
        }

        [HttpPost]
        public IActionResult Editar(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
                return View(inquilino);

            // 🔥 Verificar duplicado (excluyendo el mismo registro)
            var existeDuplicado = repoInquilino.ObtenerTodos()
                .Any(i =>
                    (i.Dni == inquilino.Dni || i.Email == inquilino.Email)
                    && i.Id != inquilino.Id);

            if (existeDuplicado)
            {
                ModelState.AddModelError("",
                    "Ya existe otro inquilino con ese DNI o email.");
                return View(inquilino);
            }

            repoInquilino.Modificacion(inquilino);

            TempData["Mensaje"] = "Inquilino modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // BAJA (Solo Administrador)
        // =========================

        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inquilino = repoInquilino.ObtenerPorId(id);
            if (inquilino == null) return NotFound();

            return View(inquilino);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var resultado = repoInquilino.Baja(id);

            // Regla de negocio: no permitir eliminar si tiene contratos asociados
            if (resultado == -1)
            {
                TempData["Error"] =
                    "No se puede eliminar el inquilino porque tiene contratos asociados.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Mensaje"] = "Inquilino eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}