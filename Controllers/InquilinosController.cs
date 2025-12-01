using Microsoft.AspNetCore.Mvc;
using InmobiliariaUlP_2025.Models;
using InmobiliariaUlP_2025.Repositories.Interfaces;

namespace InmobiliariaUlP_2025.Controllers
{
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

            repositorioInquilino.Alta(inquilino);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Editar(int id)
        {
            var inquilino = repositorioInquilino.Buscar(id);

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
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Eliminar(int id)
        {
            var inquilino = repositorioInquilino.Buscar(id);

            if (inquilino == null)
                return NotFound();

            return View(inquilino);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            repositorioInquilino.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
